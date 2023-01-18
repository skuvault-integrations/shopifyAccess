using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.Models.Product;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccessTests.Products
{
	[ TestFixture ]
	public class ProductVariantTests : BaseTests
	{
		[ Test ]
		public void GetProducts()
		{
			var products = this.Service.GetProducts( CancellationToken.None );

			products.Products.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetProductsAsync()
		{
			var products = await this.Service.GetProductsAsync( CancellationToken.None );

			products.Products.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetProductsCreatedAfterAsync()
		{
			var productsStartUtc = new DateTime( 1800, 1, 1 );

			var products = await this.Service.GetProductsCreatedAfterAsync( productsStartUtc, CancellationToken.None );

			products.Products.Count.Should().BeGreaterThan( 250 );
		}

		[ Test ]
		public async Task GetProductsCreatedAfterAsync_GetsVariationsWithUntrackedQuantity()
		{
			var products = await this.Service.GetProductsCreatedAfterAsync( DateTime.MinValue, CancellationToken.None );

			products.Products.Any( p => p.Variants.Any( v => v.InventoryManagement == InventoryManagement.Blank ) );
		}

		[ Test ]
		public async Task GetProductsThroughLocationsAsync()
		{
			var products = await this.Service.GetProductsInventoryAsync( CancellationToken.None );

			products.Products.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public void ProductVariantQuantityUpdated()
		{
			var variantToUpdate = new ShopifyProductVariantForUpdate { Id = 337095344, Quantity = 2 };
			this.Service.UpdateProductVariants( new List< ShopifyProductVariantForUpdate > { variantToUpdate }, CancellationToken.None );
		}

		[ Test ]
		public void GetAndUpdateProductQuantity()
		{
			const string sku = "testSku1";
			var products = this.Service.GetProducts( CancellationToken.None ).ToDictionary();
			const int quantity = 17;

			var productsForUpdate = new List< ShopifyInventoryLevelForUpdate >();
			foreach( var product in products )
			{
				var firstInventoryLevel = product.Value.InventoryLevels.InventoryLevels.FirstOrDefault();
				if( firstInventoryLevel == null )
					continue;

				var productForUpdate = new ShopifyInventoryLevelForUpdate
				{
					InventoryItemId = firstInventoryLevel.InventoryItemId,
					LocationId = firstInventoryLevel.LocationId,
					Quantity = firstInventoryLevel.Available
				};

				if( product.Key.Equals( sku, StringComparison.InvariantCultureIgnoreCase ) )
				{
					productForUpdate.Quantity = quantity;
					productsForUpdate.Add( productForUpdate );
					break;
				}
			}

			Service.UpdateInventoryLevels( productsForUpdate, CancellationToken.None );
			var updatedProducts = this.Service.GetProducts( CancellationToken.None ).ToDictionary();
			
			var updatedProduct = updatedProducts.FirstOrDefault( p => p.Key.Equals( sku, StringComparison.InvariantCultureIgnoreCase ) );
			updatedProduct.Value.InventoryLevels.InventoryLevels[ 0 ].Available.Should().Be( quantity );
		}

		[ Test ]
		public async Task GetAndUpdateProductAsync()
		{
			const string sku = "testsku1";
			var inventoryItem = await this.GetFirstInventoryItem( sku );
			var initialQuantity = inventoryItem.Available;
			const int quantity = 39;

			await this.Service.UpdateInventoryLevelsAsync( CreateInventoryLevelForUpdate( inventoryItem, quantity ), CancellationToken.None );
			var product = ( await this.Service.GetProductVariantsInventoryBySkusAsync( new List< string > { sku }, CancellationToken.None ) ).First();
			var newQuantity = product.InventoryLevels.InventoryLevels.First().Available;
			await this.Service.UpdateInventoryLevelsAsync( CreateInventoryLevelForUpdate( inventoryItem, initialQuantity ), CancellationToken.None );

			newQuantity.Should().Be( quantity );
		}

		[ Test ]
		public async Task WhenGetProductsCreatedAfterAsyncIsCalled_ThenProductsImagesUrlsAreExpectedWithoutQueryPart()
		{
			var products = await this.Service.GetProductsCreatedAfterAsync( DateTime.UtcNow.AddMonths( -2 ), CancellationToken.None );
			var productsWithImages = products.Products.Where( p => p.Images != null && p.Images.Any() );

			productsWithImages.Should().NotBeNullOrEmpty();

			var imagesUrlsQueries = productsWithImages.SelectMany( p => p.Images ).Select( i => new Uri( i.Src ).Query ).Where( q => !string.IsNullOrWhiteSpace( q ) );
			imagesUrlsQueries.Should().BeEmpty();
		}

		[ Test ]
		public async Task WhenGetProductsCreatedBeforeButUpdatedAfterAsyncIsCalled_ThenProductsImagesUrlsAreExpectedWithoutQueryPart()
		{
			var products = await this.Service.GetProductsCreatedBeforeButUpdatedAfterAsync( DateTime.UtcNow.AddMonths( -2 ), CancellationToken.None );
			var productsWithImages = products.Products.Where( p => p.Images != null && p.Images.Any() );

			productsWithImages.Should().NotBeNullOrEmpty();

			var imagesUrlsQueries = productsWithImages.SelectMany( p => p.Images ).Select( i => new Uri( i.Src ).Query ).Where( q => !string.IsNullOrWhiteSpace( q ) );
			imagesUrlsQueries.Should().BeEmpty();
		}

		private async Task< ShopifyInventoryLevel > GetFirstInventoryItem( string sku )
		{
			var product = ( await this.Service.GetProductVariantsInventoryBySkusAsync( new List< string > { sku }, CancellationToken.None ) ).First();
			return product.InventoryLevels.InventoryLevels.First();
		}

		private static IEnumerable< ShopifyInventoryLevelForUpdate > CreateInventoryLevelForUpdate( ShopifyInventoryLevel inventory, int setQuantity )
		{
			IEnumerable< ShopifyInventoryLevelForUpdate > inventoryLevels = new List< ShopifyInventoryLevelForUpdate >
			{
				new ShopifyInventoryLevelForUpdate
				{
					InventoryItemId = inventory.InventoryItemId,
					Quantity = setQuantity,
					LocationId = inventory.LocationId
				}
			};
			return inventoryLevels;
		}

		[ Test ]
		[ Explicit ]
		public async Task GetProductVariantsInventoryReportAsync_ReturnsCorrectReport()
		{
			// Arrange
			var products = await this.Service.GetProductsInventoryAsync( CancellationToken.None );
			var productVariants = products.ToListVariants();

			// Act
			var productVariantsReport = await this.Service.GetProductVariantsInventoryReportAsync( CancellationToken.None );

			// Assert
			this.ValidateIfEqual( productVariantsReport, productVariants );
			// This implementation is better but it works slow:
			//
			// var fieldsToCompare = new HashSet< string >() { "Id", "Sku", "InventoryItemId", "InventoryLevels", "LocationId", "Available" };
			// productVariantsReport.Should().BeEquivalentTo( productVariants, o => o.Including(
			// 	memberInfo => fieldsToCompare.Contains( memberInfo.Name ) ) );
		}

		private void ValidateIfEqual( List< ShopifyProductVariant > productVariantsReport, List< ShopifyProductVariant > productVariants )
		{
			productVariantsReport.Should().HaveCount( productVariants.Count );
			productVariantsReport.Sort( ( x, y ) => Math.Sign( x.Id - y.Id ) );
			productVariants.Sort( ( x, y ) => Math.Sign( x.Id - y.Id ) );
			for( var i = 0; i < productVariantsReport.Count; i++ )
			{
				var v1 = productVariantsReport[ i ];
				var v2 = productVariants[ i ];
				v1.Id.Should().Be( v2.Id );
				v1.InventoryItemId.Should().Be( v2.InventoryItemId );
				v1.Sku.Should().BeEquivalentTo( v2.Sku );
				v1.InventoryLevels.InventoryLevels.Should().BeEquivalentTo( v2.InventoryLevels.InventoryLevels,
					o => o.Excluding( memberInfo => memberInfo.Name.Equals( "UpdatedAt" ) ) );
			}
		}

		private int Comparison( ShopifyProductVariant x, ShopifyProductVariant y )
		{
			return 0;
		}
	}
}