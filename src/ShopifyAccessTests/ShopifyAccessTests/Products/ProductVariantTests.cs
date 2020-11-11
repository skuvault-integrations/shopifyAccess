using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LINQtoCSV;
using Netco.Logging;
using NUnit.Framework;
using ShopifyAccess;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Product;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccessTests.Products
{
	[ TestFixture ]
	public class ProductVariantTests
	{
		private readonly IShopifyFactory ShopifyFactory = new ShopifyFactory();
		private IShopifyService _service;

		[ SetUp ]
		public void Init()
		{
			Directory.SetCurrentDirectory( TestContext.CurrentContext.TestDirectory );
			const string credentialsFilePath = @"..\..\Files\ShopifyCredentials.csv";
			NetcoLogger.LoggerFactory = new ConsoleLoggerFactory();

			var cc = new CsvContext();
			var testConfig = cc.Read< TestCommandConfig >( credentialsFilePath, new CsvFileDescription { FirstLineHasColumnNames = true } ).FirstOrDefault();

			if( testConfig != null )
				this._service = this.ShopifyFactory.CreateService( new ShopifyCommandConfig( testConfig.ShopName, testConfig.AccessToken ) );
		}

		[ Test ]
		public void GetProducts()
		{
			var products = this._service.GetProducts();

			products.Products.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetProductsAsync()
		{
			var products = await this._service.GetProductsAsync( CancellationToken.None );

			products.Products.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetProductsCreatedAfterAsync()
		{
			var productsStartUtc = new DateTime( 1800, 1, 1 );

			var products = await this._service.GetProductsCreatedAfterAsync( productsStartUtc, CancellationToken.None );

			products.Products.Count.Should().BeGreaterThan( 250 );
		}

		[ Test ]
		public async Task GetProductsCreatedAfterAsync_GetsVariationsWithUntrackedQuantity()
		{
			var products = await this._service.GetProductsCreatedAfterAsync( DateTime.MinValue, CancellationToken.None );

			products.Products.Any( p => p.Variants.Any( v => v.InventoryManagement == InventoryManagement.Blank ) );
		}

		[Test ]
		public async Task GetProductVariantsBySkusAsync()
		{
			var products = await this._service.GetProductsAsync( CancellationToken.None );
			var productVariants = products.ToListVariants();

			// take 10% random variants
			var filteredProductVariants = productVariants.OrderBy( v => Guid.NewGuid() ).Take( productVariants.Count / 10 ).ToList();
			var filteredSkus = filteredProductVariants.Select( v => v.Sku.ToUpperInvariant() );

			var variants = await this._service.GetProductVariantsInventoryBySkusAsync( filteredSkus, CancellationToken.None );
			var expectedVariants = productVariants.Where( v => filteredSkus.Contains( v.Sku.ToUpperInvariant() ) ).ToList();
			variants.ShouldBeEquivalentTo( expectedVariants );
		}

		[ Test ]
		public async Task GetProductsThroughLocationsAsync()
		{
			var products = await this._service.GetProductsInventoryAsync( CancellationToken.None );

			products.Products.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public void ProductVariantQuantityUpdated()
		{
			var variantToUpdate = new ShopifyProductVariantForUpdate { Id = 337095344, Quantity = 2 };
			this._service.UpdateProductVariants( new List< ShopifyProductVariantForUpdate > { variantToUpdate } );
		}

		[ Test ]
		public void GetAndUpdateProduct()
		{
			var products = this._service.GetProducts().ToDictionary();

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

				if( product.Key.Equals( "T-BLA-S", StringComparison.InvariantCultureIgnoreCase ) )
				{
					productForUpdate.Quantity = 17;
					productsForUpdate.Add( productForUpdate );
				}
				
			}

			_service.UpdateInventoryLevels( productsForUpdate );
			var updatedProducts = this._service.GetProducts().ToDictionary();
			
			products.Should().Equal( updatedProducts );
		}

		[ Test ]
		public async Task GetAndUpdateProductAsync()
		{
			const string sku = "testsku1";
			var inventoryItem = await this.GetFirstInventoryItem( sku );
			var initialQuantity = inventoryItem.Available;
			const int quantity = 39;

			await this._service.UpdateInventoryLevelsAsync( CreateInventoryLevelForUpdate( inventoryItem, quantity ) );
			var product = ( await this._service.GetProductVariantsInventoryBySkusAsync( new List< string > { sku }, CancellationToken.None ) ).First();
			var newQuantity = product.InventoryLevels.InventoryLevels.First().Available;
			await this._service.UpdateInventoryLevelsAsync( CreateInventoryLevelForUpdate( inventoryItem, initialQuantity ) );

			newQuantity.Should().Be( quantity );
		}

		[ Test ]
		public async Task WhenGetProductsCreatedAfterAsyncIsCalled_ThenProductsImagesUrlsAreExpectedWithoutQueryPart()
		{
			var products = await this._service.GetProductsCreatedAfterAsync( DateTime.UtcNow.AddMonths( -2 ), CancellationToken.None );
			var productsWithImages = products.Products.Where( p => p.Images != null && p.Images.Any() );

			productsWithImages.Should().NotBeNullOrEmpty();

			var imagesUrlsQueries = productsWithImages.SelectMany( p => p.Images ).Select( i => new Uri( i.Src ).Query ).Where( q => !string.IsNullOrWhiteSpace( q ) );
			imagesUrlsQueries.Should().BeEmpty();
		}

		[ Test ]
		public async Task WhenGetProductsCreatedBeforeButUpdatedAfterAsyncIsCalled_ThenProductsImagesUrlsAreExpectedWithoutQueryPart()
		{
			var products = await this._service.GetProductsCreatedBeforeButUpdatedAfterAsync( DateTime.UtcNow.AddMonths( -2 ), CancellationToken.None );
			var productsWithImages = products.Products.Where( p => p.Images != null && p.Images.Any() );

			productsWithImages.Should().NotBeNullOrEmpty();

			var imagesUrlsQueries = productsWithImages.SelectMany( p => p.Images ).Select( i => new Uri( i.Src ).Query ).Where( q => !string.IsNullOrWhiteSpace( q ) );
			imagesUrlsQueries.Should().BeEmpty();
		}

		private async Task< ShopifyInventoryLevel > GetFirstInventoryItem( string sku )
		{
			var product = ( await this._service.GetProductVariantsInventoryBySkusAsync( new List< string > { sku }, CancellationToken.None ) ).First();
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
	}
}