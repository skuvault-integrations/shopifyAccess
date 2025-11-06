using System.Collections.Generic;
using NUnit.Framework;
using ShopifyAccess.GraphQl.Models.Products;
using ShopifyAccess.GraphQl.Models.Responses;

namespace ShopifyAccessTests.GraphQl.Models.Responses
{
	public class GetProductsResponseExtensionsTests
	{
		[ Test ]
		public void ToShopifyProducts_ReturnsEmptyShopifyProducts_WhenNullResponseProductsPassedIn()
		{
			List< Product > responseProducts = null;

			var result = responseProducts.ToShopifyProducts( new Dictionary< long, List< ProductVariant > >() );

			Assert.That( result.Products, Has.Count.EqualTo( 0 ) );
		}

		[ Test ]
		public void ToShopifyProducts_ReturnsEmptyShopifyProducts_WhenNoVariantsPassedIn()
		{
			var responseProducts = new List< Product >();

			var result = responseProducts.ToShopifyProducts( new Dictionary< long, List< ProductVariant > >() );

			Assert.That( result.Products, Has.Count.EqualTo( 0 ) );
		}
		
		//TODO GUARD-3946 Add tests ToShopifyProducts tests
		//ToShopifyProducts_ReturnsProducts_WhenSomePassedIn()
		//ToShopifyProducts_ReturnsProductsWithVariants_WhenProductIdsMatchToVariantsParentProductIds()
		//ToShopifyProducts_ReturnsProductsWithoutVariants_WhenProductIdsDoNotMatchToVariantsParentProductIds()
	}
}