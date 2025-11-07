using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ShopifyAccess.GraphQl;
using ShopifyAccess.GraphQl.Helpers;
using ShopifyAccess.GraphQl.Queries;

namespace ShopifyAccessTests.GraphQl.Queries
{
	[ TestFixture ]
	//TODO GUARD-3954 On feature cleanup remove tests with Legacy in the name
	public class QueryBuilderTests
	{
		private static readonly Randomizer _randomizer = new Randomizer();

		[ Test ]
		public void GetCurrentBulkOperationStatusRequest_EscapeTabCharacters()
		{
			// Act
			var request = QueryBuilder.GetCurrentBulkOperationStatusRequest();

			// Assert
			request.Should().NotContain( "\t" );
		}

		[ Test ]
		public void GetCurrentBulkOperationStatusRequest_ReturnsRequestQuery()
		{
			// Act
			var request = QueryBuilder.GetCurrentBulkOperationStatusRequest();

			// Assert
			request.Should().Contain( "currentBulkOperation" );
		}

		[ Test ]
		public void GetBulkOperationStatusByIdRequest_ReturnsRequestQuery()
		{
			// Arrange
			var gid = "gid://shopify/InventoryItem/123456789";

			// Act
			var request = QueryBuilder.GetBulkOperationStatusByIdRequest( gid );

			// Assert
			request.Should().Contain( "... on BulkOperation" );
			request.Should().Contain( gid );
		}

		[ Test ]
		public void GetProductVariantInventoryBySkuRequest_ReturnsRequestQuery()
		{
			// Arrange
			var sku = "TestSku1";
			var after = "111222333444555666777888999000";
			var locationsCount = 132;

			// Act
			var request = QueryBuilder.GetProductVariantInventoryBySkuRequest( sku, after, locationsCount );

			// Assert
			request.Should().Contain( sku );
			request.Should().Contain( after );
			request.Should().Contain( locationsCount.ToString() );
		}
		
		[ Test ]
		public void GetProductVariantInventoryBySkuRequest_EscapesSkuCharacters()
		{
			// Arrange
			var sku = @"TEST 2649 1 ' "" \ ! @ # $ % ^";
			var escapedSku = @"TEST 2649 1 ' \\\"" \\\\ ! @ # $ % ^";
			var after = "111222333444555666777888999000";
			var locationsCount = 132;

			// Act
			var request = QueryBuilder.GetProductVariantInventoryBySkuRequest( sku, after, locationsCount );

			// Assert
			request.Should().Contain( escapedSku );
			request.Should().Contain( after );
			request.Should().Contain( locationsCount.ToString() );
		}

		[ Test ]
		public void GetProductVariantInventoryBySkuRequest_ThrowsArgumentOutOfRangeException_WhenLocationsCountExceedsLimit()
		{
			// Arrange
			var sku = "TestSku1";
			var after = "111222333444555666777888999000";
			var locationsCount = 251;

			// Act
			var action = new Action( () => QueryBuilder.GetProductVariantInventoryBySkuRequest( sku, after, locationsCount ) );

			// Assert
			action.Should().Throw< ArgumentException >();
		}

		[ Test ]
		public void GetReportRequest_ReturnsGetProductVariantsInventoryReportQuery_WhenProductVariantsInventoryReportType()
		{
			// Arrange
			var reportType = ReportType.ProductVariantsInventory;

			// Act
			var request = QueryBuilder.GetReportRequest( reportType );

			// Assert
			request.Should().Contain( "bulkOperationRunQuery" );
		}

		[ Test ]
		public void GetReportRequest_ThrowsArgumentOutOfRangeException_WhenReportTypeIsUnknown()
		{
			// Arrange
			var reportType = ReportType.Unknown;

			// Act
			var action = new Action( () => QueryBuilder.GetReportRequest( reportType ) );

			// Assert
			action.Should().Throw< ArgumentException >();
		}

		[ Test ]
		public void GetProductsCreatedOnOrAfterRequestLegacy_ReturnsRequestQuery()
		{
			var createdAtMinUtc = new DateTime( _randomizer.NextLong( DateTime.UtcNow.Ticks ) );
			var nextCursor = _randomizer.GetString();
			var productsPerPage = ( int )_randomizer.NextUInt( 1, 250 );
			
			var result = QueryBuilder.GetProductsCreatedOnOrAfterRequestLegacy( createdAtMinUtc, nextCursor, productsPerPage );
			
			Assert.Multiple(() => 
			{
				Assert.That( result.Contains( $"created_at:>='{createdAtMinUtc.ToIso8601()}'" ), Is.True );
				Assert.That( result.Contains( nextCursor ), Is.True );
				Assert.That( result.Contains( productsPerPage.ToString() ), Is.True );
			});
		}

		[ Test ]
		public void GetProductsCreatedOnOrAfterRequest_ReturnsRequestQuery()
		{
			var createdAtMinUtc = new DateTime( _randomizer.NextLong( DateTime.UtcNow.Ticks ) );
			var nextCursor = _randomizer.GetString();
			var productsPerPage = ( int )_randomizer.NextUInt( 1, 250 );

			var result = QueryBuilder.GetProductsCreatedOnOrAfterRequest( createdAtMinUtc, nextCursor, productsPerPage );

			Assert.Multiple(() => 
			{
				Assert.That( result.Contains( $"created_at:>='{createdAtMinUtc.ToIso8601()}'" ), Is.True );
				Assert.That( result.Contains( nextCursor ), Is.True );
				Assert.That( result.Contains( productsPerPage.ToString() ), Is.True );
			});
		}

		[ Test ]
		public void GetProductsCreatedOnOrAfterRequestLegacy_ThrowsArgumentOutOfRangeException_WhenProductsPerPageExceedsMaximum()
		{
			var tooManyProductsPerPage = 251;
			
			Assert.Throws< ArgumentOutOfRangeException > ( () => QueryBuilder.GetProductsCreatedOnOrAfterRequestLegacy( DateTime.MinValue, after: null, tooManyProductsPerPage ) );
		}

		[ Test ]
		public void GetProductsCreatedOnOrAfterRequest_ThrowsArgumentOutOfRangeException_WhenProductsPerPageExceedsMaximum()
		{
			var tooManyProductsPerPage = 251;

			Assert.Throws< ArgumentOutOfRangeException > ( () => QueryBuilder.GetProductsCreatedOnOrAfterRequest( DateTime.MinValue, after: null, tooManyProductsPerPage ) );
		}

		[ Test ]
		public void GetProductsCreatedBeforeButUpdatedAfterLegacy_ReturnsRequestQuery()
		{
			var createdAtMaxAndUpdatedAtMinUtc = new DateTime( _randomizer.NextLong( DateTime.UtcNow.Ticks ) );
			var nextCursor = _randomizer.GetString();
			var productsPerPage = ( int )_randomizer.NextUInt( 1, 250 );
			
			var result = QueryBuilder.GetProductsCreatedBeforeButUpdatedAfterLegacy( createdAtMaxAndUpdatedAtMinUtc, nextCursor, productsPerPage );
			
			Assert.Multiple(() => 
			{
				Assert.That( result.Contains( $"created_at:<='{createdAtMaxAndUpdatedAtMinUtc.ToIso8601()}' AND updated_at:>='{createdAtMaxAndUpdatedAtMinUtc.ToIso8601()}'" ), Is.True );
				Assert.That( result.Contains( nextCursor ), Is.True );
				Assert.That( result.Contains( productsPerPage.ToString() ), Is.True );
			});
		}

		[ Test ]
		public void GetProductsCreatedBeforeButUpdatedAfter_ReturnsRequestQuery()
		{
			var createdAtMaxAndUpdatedAtMinUtc = new DateTime( _randomizer.NextLong( DateTime.UtcNow.Ticks ) );
			var nextCursor = _randomizer.GetString();
			var productsPerPage = ( int )_randomizer.NextUInt( 1, 250 );

			var result = QueryBuilder.GetProductsCreatedBeforeButUpdatedAfter( createdAtMaxAndUpdatedAtMinUtc, nextCursor, productsPerPage );

			Assert.Multiple(() => 
			{
				Assert.That( result.Contains( $"created_at:<='{createdAtMaxAndUpdatedAtMinUtc.ToIso8601()}' AND updated_at:>='{createdAtMaxAndUpdatedAtMinUtc.ToIso8601()}'" ), Is.True );
				Assert.That( result.Contains( nextCursor ), Is.True );
				Assert.That( result.Contains( productsPerPage.ToString() ), Is.True );
			});
		}

		[ Test ]
		public void GetProductsCreatedBeforeButUpdatedAfterLegacy_ThrowsArgumentOutOfRangeException_WhenProductsPerPageExceedsMaximum()
		{
			var tooManyProductsPerPage = 251;
			
			Assert.Throws< ArgumentOutOfRangeException > ( () => QueryBuilder.GetProductsCreatedBeforeButUpdatedAfterLegacy( DateTime.MinValue, after: null, tooManyProductsPerPage ) );
		}

		[ Test ]
		public void GetProductsCreatedBeforeButUpdatedAfter_ThrowsArgumentOutOfRangeException_WhenProductsPerPageExceedsMaximum()
		{
			var tooManyProductsPerPage = 251;

			Assert.Throws< ArgumentOutOfRangeException > ( () => QueryBuilder.GetProductsCreatedBeforeButUpdatedAfter( DateTime.MinValue, after: null, tooManyProductsPerPage ) );
		}


		[ Test ]
		public void GetAllProductVariants_ReturnsRequestQuery()
		{
			var nextCursor = _randomizer.GetString();
			var productsPerPage = ( int )_randomizer.NextUInt( 1, 250 );
			
			var result = QueryBuilder.GetAllProductVariants( nextCursor, productsPerPage );
			
			Assert.Multiple(() => 
			{
				Assert.That( result.Contains( nextCursor ), Is.True );
				Assert.That( result.Contains( productsPerPage.ToString() ), Is.True );
			});
		}
		
		[ Test ]
		public void GetAllProductVariants_ThrowsArgumentOutOfRangeException_WhenProductsPerPageExceedsMaximum()
		{
			var tooManyProductsPerPage = 251;

			Assert.Throws< ArgumentOutOfRangeException > ( () => QueryBuilder.GetAllProductVariants( after: null, tooManyProductsPerPage ) );
		}

		[ Test ]
		public void GetProductVariantsByProductIds_ThrowsArgumentOutOfRangeException_WhenVariantsPerPageExceedsMaximum()
		{
			const int tooManyVariantsPerPage = QueryBuilder.MaxItemsPerResponse + 1;
			var productIds = new []{ _randomizer.NextLong() };

			Assert.Throws< ArgumentOutOfRangeException > ( () => 
				QueryBuilder.GetProductVariantsByProductIds( productIds, variantsPerPage: tooManyVariantsPerPage ) );
		}

		[ Test ]
		public void GetProductVariantsByProductIds_ThrowsArgumentOutOfRangeException_WhenTooManyProductIdsPassedIn()
		{
			var tooManyProductIds = Enumerable.Range( 0, QueryBuilder.MaxItemsPerResponse + 1 )
				.Select( _ => _randomizer.NextLong() ).ToList();

			Assert.Throws< ArgumentOutOfRangeException > ( () => 
				QueryBuilder.GetProductVariantsByProductIds( tooManyProductIds ) );
		}

		[ Test ]
		public void GetProductVariantsByProductIds_ReturnsRequestQuery()
		{
			var productId1 = _randomizer.NextLong();
			var productId2 = _randomizer.NextLong();
			var productIds = new [] { productId1, productId2 };
			var after = _randomizer.GetString();
			var variantsPerPage = ( int )_randomizer.NextUInt( 1, QueryBuilder.MaxItemsPerResponse );

			var result = QueryBuilder.GetProductVariantsByProductIds( productIds, after, variantsPerPage );

			Assert.Multiple( () => {
				Assert.That( result.Contains( $"product_ids:{productId1},{productId2}" ), Is.True );
				Assert.That( result.Contains( after ), Is.True );
				Assert.That( result.Contains( variantsPerPage.ToString() ), Is.True );
			} );
		}
	}
}