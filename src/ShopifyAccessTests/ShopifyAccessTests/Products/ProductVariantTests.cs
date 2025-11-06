using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess;
using ShopifyAccess.GraphQl.Helpers;
using ShopifyAccess.Models;
using ShopifyAccess.Models.Product;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccessTests.Products
{
	[ TestFixture ]
	public class ProductVariantTests : BaseTests
	{
		private static readonly Mark _mark = Mark.Create;

		[ Test ]
		[ Explicit ]
		//TODO GUARD-3954 Remove on feature cleanup
		public async Task GetProductsCreatedAfterLegacyAsync()
		{
			var productsStartUtc = DateTime.Parse( "2025-01-06T17:14:34Z" ); 

			var products = await this.Service.GetProductsCreatedAfterLegacyAsync( productsStartUtc, CancellationToken.None, _mark );

			Assert.That( products.Products, Is.Not.Empty );
		}

		[ Test ]
		[ Explicit ]
		//TODO GUARD-3946 Run test
		public async Task GetProductsCreatedAfterAsync()
		{
			var productsStartUtc = DateTime.Parse( "2025-01-06T17:14:34Z" ); 

			var products = await this.Service.GetProductsCreatedAfterAsync( productsStartUtc, CancellationToken.None, _mark );

			Assert.That( products.Products, Is.Not.Empty );
		}

		[ Test ]
		[ Explicit ]
		//TODO GUARD-3954 Remove on feature cleanup
		public async Task GetProductsCreatedBeforeButUpdatedAfterLegacyAsync()
		{
			var productsStartUtc = DateTime.Parse( "2025-01-13T20:52:49Z" );

			var products = await this.Service.GetProductsCreatedBeforeButUpdatedAfterLegacyAsync( productsStartUtc, CancellationToken.None, _mark );

			Assert.That( products.Products, Is.Not.Empty );
		}

		[ Test ]
		[ Explicit ]
		//TODO GUARD-3946 Rerun tests
		public async Task GetProductsCreatedBeforeButUpdatedAfterAsync()
		{
			var productsStartUtc = DateTime.Parse( "2025-01-13T20:52:49Z" );

			var products = await this.Service.GetProductsCreatedBeforeButUpdatedAfterAsync( productsStartUtc, CancellationToken.None, _mark );

			Assert.That( products.Products, Is.Not.Empty );
		}

		[ Test ]
		public async Task GetProductVariantsInventoryAsync()
		{
			var productVariants = await this.Service.GetProductVariantsInventoryAsync( CancellationToken.None, _mark );

			Assert.That( productVariants, Is.Not.Empty );
		}

		[ Test ]
		public async Task GetAndUpdateProductAsync()
		{
			const string sku = "testsku1";
			var inventoryItem = await this.GetFirstInventoryItem( sku );
			var initialQuantity = inventoryItem.Available.Value;
			const int quantity = 39;

			await this.Service.UpdateInventoryLevelsAsync( CreateInventoryLevelForUpdate( inventoryItem, quantity ), CancellationToken.None, _mark );
			var product = ( await this.Service.GetProductVariantsInventoryBySkusAsync( new List< string > { sku }, CancellationToken.None, _mark ) ).First();
			var newQuantity = product.InventoryLevels.InventoryLevels.First().Available;
			await this.Service.UpdateInventoryLevelsAsync( CreateInventoryLevelForUpdate( inventoryItem, initialQuantity ), CancellationToken.None, _mark );

			newQuantity.Should().Be( quantity );
		}

		[ Test ]
		//TODO GUARD-3954 Remove on feature cleanup
		public async Task WhenGetProductsCreatedAfterLegacyAsyncIsCalled_ThenProductsImagesUrlsAreExpectedWithoutQueryPart()
		{
			var dateFrom = new DateTime( 2021, 6, 1 );
			var products = await this.Service.GetProductsCreatedAfterLegacyAsync( dateFrom, CancellationToken.None, _mark );
			var productsWithImages = products.Products.Where( p => p.Images != null && p.Images.Any() );

			productsWithImages.Should().NotBeNullOrEmpty();

			var imagesUrlsQueries = productsWithImages.SelectMany( p => p.Images ).Select( i => new Uri( i.Src ).Query ).Where( q => !string.IsNullOrWhiteSpace( q ) );
			imagesUrlsQueries.Should().BeEmpty();
		}

		[ Test ]
		//TODO GUARD-3946 Run test
		public async Task WhenGetProductsCreatedAfterAsyncIsCalled_ThenProductsImagesUrlsAreExpectedWithoutQueryPart()
		{
			var dateFrom = new DateTime( 2021, 6, 1 );
			var products = await this.Service.GetProductsCreatedAfterAsync( dateFrom, CancellationToken.None, _mark );
			var productsWithImages = products.Products.Where( p => p.Images != null && p.Images.Any() );

			productsWithImages.Should().NotBeNullOrEmpty();

			var imagesUrlsQueries = productsWithImages.SelectMany( p => p.Images ).Select( i => new Uri( i.Src ).Query ).Where( q => !string.IsNullOrWhiteSpace( q ) );
			imagesUrlsQueries.Should().BeEmpty();
		}

		[ Test ]
		//TODO GUARD-3954 Remove on feature cleanup
		public async Task WhenGetProductsCreatedBeforeButUpdatedAfterLegacyAsyncIsCalled_ThenProductsImagesUrlsAreExpectedWithoutQueryPart()
		{
			var dateFrom = new DateTime( 2023, 6, 1 );
			var products = await this.Service.GetProductsCreatedBeforeButUpdatedAfterLegacyAsync( dateFrom, CancellationToken.None, _mark );
			var productsWithImages = products.Products.Where( p => p.Images != null && p.Images.Any() );

			productsWithImages.Should().NotBeNullOrEmpty();

			var imagesUrlsQueries = productsWithImages.SelectMany( p => p.Images ).Select( i => new Uri( i.Src ).Query ).Where( q => !string.IsNullOrWhiteSpace( q ) );
			imagesUrlsQueries.Should().BeEmpty();
		}

		[ Test ]
		//TODO GUARD-3946 Run test
		public async Task WhenGetProductsCreatedBeforeButUpdatedAfterAsyncIsCalled_ThenProductsImagesUrlsAreExpectedWithoutQueryPart()
		{
			var dateFrom = new DateTime( 2023, 6, 1 );
			var products = await this.Service.GetProductsCreatedBeforeButUpdatedAfterAsync( dateFrom, CancellationToken.None, _mark );
			var productsWithImages = products.Products.Where( p => p.Images != null && p.Images.Any() );

			productsWithImages.Should().NotBeNullOrEmpty();

			var imagesUrlsQueries = productsWithImages.SelectMany( p => p.Images ).Select( i => new Uri( i.Src ).Query ).Where( q => !string.IsNullOrWhiteSpace( q ) );
			imagesUrlsQueries.Should().BeEmpty();
		}

		[ Test ]
		[ Explicit( "Calls the real API, and these productIds might no longer exist. Thus the result.Count assert might fail" ) ]
		public async Task GetProductVariantsByProductIdsAsync_ReturnsVariantsForPassedInProductIds_WhenMultiplePages()
		{
			var productIds = GetExistingProductIds();
			const int simulateSmallPageSize = 1;

			var result = ( await ( ( ShopifyService )this.Service ).GetProductVariantsByProductIdsAsync( productIds, _mark, CancellationToken.None, variantsPerPage : simulateSmallPageSize ) )
				.ToList();

			Assert.Multiple(() => {
				Assert.That( result.Count, Is.GreaterThan( 1 ) );
				var resultProductIds = result.Select( x => GraphQlIdParser.Product.GetId( x.Product.Id ) ).Distinct();
				//Only returns variants whose parent product is one of productIds
				Assert.That( resultProductIds.All( x => productIds.Contains( x )  ), Is.True );
			} );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetProductVariantsByProductIdsAsync_ReturnsVariants_WhenMaxNumberOfProductIdsSent()
		{
			var productIds = GetExistingProductIds();
			AppendRandomProductIds( productIds, ShopifyService.RequestMaxLimit );

			var result = ( await ( ( ShopifyService )this.Service ).GetProductVariantsByProductIdsAsync( productIds, _mark, CancellationToken.None ) )
				.ToList();

			Assert.That( result.Count, Is.GreaterThan( 1 ) );
		}

		/// <summary>
		/// Appends random(-ish) productIds, so that the <param name="productIds" /> array is <param name="maxProductIds" /> long at the end.
		/// </summary>
		/// <param name="productIds"></param>
		/// <param name="maxProductIds"></param>
		private static void AppendRandomProductIds( List< long > productIds, int maxProductIds )
		{
			var nextFakeProductId = productIds.Last() + 1;
			for( var i = productIds.Count; i < maxProductIds; i++ )
			{
				productIds.Add( nextFakeProductId++ );
			}
		}

		/// <summary>
		/// Get productIds that exist in the qa-skuvault-development Shopify test store 
		/// </summary>
		/// <returns></returns>
		private static List< long > GetExistingProductIds()
		{
			return new List< long > { 9729995637050, 9733037949242, 9779221725498, 9943945019706, 9943946428730 };
		}

		[ Test ]
		[ Explicit ]
		public async Task GetProductVariantsInventoryReportAsync_ReturnsCorrectReport()
		{
			// Arrange
			var productVariants = await this.Service.GetProductVariantsInventoryAsync( CancellationToken.None, _mark );

			// Act
			var productVariantsReport = await this.Service.GetProductVariantsInventoryReportAsync( CancellationToken.None, _mark );

			// Assert
			this.ValidateIfEqual( productVariantsReport, productVariants );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetProductVariantsInventoryReportBySkusAsync_ReturnsCorrectData_WhenSingleSku_AndSkuContainsSpecialCharacters()
		{
			// Arrange
			var skus = new[] { "AUTO_TEST 2649 1 ' \" \\ ! @ # $ % ^" };
			var products = await this.Service.GetProductVariantsInventoryBySkusAsync( skus, CancellationToken.None, _mark );

			// Act
			var productVariants = await this.Service.GetProductVariantsInventoryReportBySkusAsync( skus, CancellationToken.None, _mark );

			// Assert
			productVariants.Should().NotBeEmpty();
			this.ValidateIfEqual( productVariants, products );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetProductVariantsInventoryBySkusAsync_ReturnsCorrectData_WhenSingleSku_AndMoreThanOneVariantForTheSku()
		{
			// Arrange
			var skus = new[] { "testSKU1" };
			var products = await this.Service.GetProductVariantsInventoryBySkusAsync( skus, CancellationToken.None, _mark );

			// Act
			var productVariants = await this.Service.GetProductVariantsInventoryReportBySkusAsync( skus, CancellationToken.None, _mark );

			// Assert
			productVariants.Count.Should().BeGreaterThan( 0 );
			this.ValidateIfEqual( productVariants, products );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetProductVariantsInventoryReportBySkusAsync_ReturnsEmptyList_WhenWrongSku()
		{
			// Arrange
			var skus = new[] { "wrong SKU" };

			// Act
			var productVariants = await this.Service.GetProductVariantsInventoryReportBySkusAsync( skus, CancellationToken.None, _mark );

			// Assert
			productVariants.Should().BeEmpty();
		}

		[ Test ]
		[ Explicit ]
		public async Task GetProductVariantsInventoryReportBySkusAsync_ReturnsCorrectData_WhenMultipleSkusRequested()
		{
			// Arrange
			var countToCompare = 60;
			var productVariants = await this.Service.GetProductVariantsInventoryAsync( CancellationToken.None, _mark );
			productVariants = productVariants.Take( countToCompare ).ToList();
			var skus = productVariants.Select( v => v.Sku );

			// Act
			var productVariantsFromReport = await this.Service.GetProductVariantsInventoryReportBySkusAsync( skus, CancellationToken.None, _mark );

			// Assert
			this.ValidateIfEqual( productVariantsFromReport, productVariants );
		}

		private async Task< ShopifyInventoryLevel > GetFirstInventoryItem( string sku )
		{
			var product = ( await this.Service.GetProductVariantsInventoryBySkusAsync( new List< string > { sku }, CancellationToken.None, _mark ) ).First();
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
	}
}