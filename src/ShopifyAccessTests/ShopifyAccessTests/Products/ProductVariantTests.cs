using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.Models;
using ShopifyAccess.Models.Product;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccessTests.Products
{
	[ TestFixture ]
	public class ProductVariantTests : BaseTests
	{
		private static readonly Mark _mark = Mark.Create;
		
		//TODO GUARD-3717 [Refactor] Move to GraphQL folder all tests of GetProductsCreatedAfterAsync, GetProductsCreatedBeforeButUpdatedAfterAsync, GetProductVariantsInventoryReportBySkusAsync
		[ Test ]
		[ Explicit ]
		public async Task GetProductsCreatedAfterAsync()
		{
			var productsStartUtc = DateTime.Parse( "2025-01-06T17:14:34Z" ); 

			var products = await this.Service.GetProductsCreatedAfterAsync( productsStartUtc, CancellationToken.None, _mark );

			Assert.That( products.Products, Is.Not.Empty );
		}
		
		[ Test ]
		[ Explicit ]
		public async Task GetProductsCreatedBeforeButUpdatedAfterAsync()
		{
			var productsStartUtc = DateTime.Parse( "2025-01-13T20:52:49Z" );

			var products = await this.Service.GetProductsCreatedBeforeButUpdatedAfterAsync( productsStartUtc, CancellationToken.None, _mark );

			Assert.That( products.Products, Is.Not.Empty );
		}

		[ Test ]
		public async Task GetProductsCreatedAfterAsync_GetsVariationsWithUntrackedQuantity()
		{
			var products = await this.Service.GetProductsCreatedAfterAsync( DateTime.MinValue, CancellationToken.None, _mark );

			products.Products.Any( p => p.Variants.Any( v => v.InventoryManagement == InventoryManagementEnum.Blank ) );
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

			await this.Service.UpdateInventoryLevelsAsync( CreateInventoryLevelForUpdate( inventoryItem, quantity ), CancellationToken.None );
			var product = ( await this.Service.GetProductVariantsInventoryBySkusAsync( new List< string > { sku }, CancellationToken.None ) ).First();
			var newQuantity = product.InventoryLevels.InventoryLevels.First().Available;
			await this.Service.UpdateInventoryLevelsAsync( CreateInventoryLevelForUpdate( inventoryItem, initialQuantity ), CancellationToken.None );

			newQuantity.Should().Be( quantity );
		}

		[ Test ]
		public async Task WhenGetProductsCreatedAfterAsyncIsCalled_ThenProductsImagesUrlsAreExpectedWithoutQueryPart()
		{
			var dateFrom = new DateTime( 2021, 6, 1 );
			var products = await this.Service.GetProductsCreatedAfterAsync( dateFrom, CancellationToken.None );
			var productsWithImages = products.Products.Where( p => p.Images != null && p.Images.Any() );

			productsWithImages.Should().NotBeNullOrEmpty();

			var imagesUrlsQueries = productsWithImages.SelectMany( p => p.Images ).Select( i => new Uri( i.Src ).Query ).Where( q => !string.IsNullOrWhiteSpace( q ) );
			imagesUrlsQueries.Should().BeEmpty();
		}

		[ Test ]
		public async Task WhenGetProductsCreatedBeforeButUpdatedAfterAsyncIsCalled_ThenProductsImagesUrlsAreExpectedWithoutQueryPart()
		{
			var dateFrom = new DateTime( 2023, 6, 1 );
			var products = await this.Service.GetProductsCreatedBeforeButUpdatedAfterAsync( dateFrom, CancellationToken.None );
			var productsWithImages = products.Products.Where( p => p.Images != null && p.Images.Any() );

			productsWithImages.Should().NotBeNullOrEmpty();

			var imagesUrlsQueries = productsWithImages.SelectMany( p => p.Images ).Select( i => new Uri( i.Src ).Query ).Where( q => !string.IsNullOrWhiteSpace( q ) );
			imagesUrlsQueries.Should().BeEmpty();
		}

		[ Test ]
		[ Explicit ]
		public async Task GetProductVariantsInventoryReportAsync_ReturnsCorrectReport()
		{
			// Arrange
			var productVariants = await this.Service.GetProductVariantsInventoryAsync( CancellationToken.None, _mark );

			// Act
			var productVariantsReport = await this.Service.GetProductVariantsInventoryReportAsync( CancellationToken.None );

			// Assert
			this.ValidateIfEqual( productVariantsReport, productVariants );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetProductVariantsInventoryReportBySkusAsync_ReturnsCorrectData_WhenSingleSku_AndSkuContainsSpecialCharacters()
		{
			// Arrange
			var skus = new[] { "AUTO_TEST 2649 1 ' \" \\ ! @ # $ % ^" };
			var products = await this.Service.GetProductVariantsInventoryBySkusAsync( skus, CancellationToken.None );

			// Act
			var productVariants = await this.Service.GetProductVariantsInventoryReportBySkusAsync( skus, CancellationToken.None );

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
			var products = await this.Service.GetProductVariantsInventoryBySkusAsync( skus, CancellationToken.None );

			// Act
			var productVariants = await this.Service.GetProductVariantsInventoryReportBySkusAsync( skus, CancellationToken.None );

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
			var productVariants = await this.Service.GetProductVariantsInventoryReportBySkusAsync( skus, CancellationToken.None );

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