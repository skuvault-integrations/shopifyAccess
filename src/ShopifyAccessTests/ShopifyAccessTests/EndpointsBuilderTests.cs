using System;
using NUnit.Framework;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Product;

namespace ShopifyAccessTests
{
	[ TestFixture ]
	public class EndpointsBuilderTests
	{
		[ Test ]
		public void AppendGetProductsAfterDateEndpoint_GetCreatedAfter()
		{
			var productsDateFilter = new ProductsDateFilter
			{
				FilterType = FilterType.CreatedAfter,
				ProductsStartUtc = new DateTime( 2000, 10, 10 )
			};
			const string initialEndpoint = "?a=b";

			var endpoint = ShopifyAccess.Services.EndpointsBuilder.AppendGetProductsFilteredByDateEndpoint( productsDateFilter, initialEndpoint );

			var expected = string.Format("&{0}={1}", 
				ShopifyProductCommandEndpointName.ProductDateCreatedAfter.Name, DateTime.SpecifyKind( productsDateFilter.ProductsStartUtc, DateTimeKind.Utc ).ToString( "o" ));
			Assert.AreEqual( expected, endpoint );
		}

		[ Test ]
		public void AppendGetProductsAfterDateEndpoint_GetCreatedBeforeUpdatedAfter()
		{
			var productsDateFilter = new ProductsDateFilter
			{
				FilterType = FilterType.CreatedBeforeUpdatedAfter,
				ProductsStartUtc = new DateTime( 2000, 10, 10 )
			};
			const string initialEndpoint = "?a=b";

			var endpoint = ShopifyAccess.Services.EndpointsBuilder.AppendGetProductsFilteredByDateEndpoint( productsDateFilter, initialEndpoint );

			var expected = string.Format("&{0}={1}&{2}={3}", 
				ShopifyProductCommandEndpointName.ProductDateCreatedBefore.Name, DateTime.SpecifyKind( productsDateFilter.ProductsStartUtc, DateTimeKind.Utc ).ToString( "o" ),
				ShopifyProductCommandEndpointName.ProductDateUpdatedAfter.Name, DateTime.SpecifyKind( productsDateFilter.ProductsStartUtc, DateTimeKind.Utc ).ToString( "o" ) );
			Assert.AreEqual( expected, endpoint );
		}
	}
}
