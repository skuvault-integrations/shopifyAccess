using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ShopifyAccess.GraphQl.Models.Products;
using ShopifyAccess.GraphQl.Models.Responses;
using ShopifyAccessTests.GraphQl.Helpers;

namespace ShopifyAccessTests.GraphQl.Models.Responses
{
	public class GetProductsResponseExtensionsTests
	{
		private static readonly Randomizer _randomizer = new Randomizer();

		[ Test ]
		public void ToShopifyProducts_ReturnsEmptyShopifyProducts_WhenNullResponseProductsPassedIn()
		{
			List< Product > responseProducts = null;

			var result = responseProducts.ToShopifyProducts( new Dictionary< long, List< ProductVariant > >() );

			Assert.That( result.Products, Has.Count.EqualTo( 0 ) );
		}

		[ Test ]
		public void ToShopifyProducts_ReturnsShopifyProductWithoutVariants_WhenNoVariantsPassedIn()
		{
			var responseProducts = new List< Product > {
				new Product { Title = _randomizer.GetString() }
			};

			var result = responseProducts.ToShopifyProducts( new Dictionary< long, List< ProductVariant > >() );

			Assert.Multiple(() => {
				Assert.That( result.Products, Has.Count.EqualTo( responseProducts.Count ) );
				Assert.That( result.Products.Single().Variants, Is.Empty );
			});
		}

		[ Test ]
		public void ToShopifyProducts_ReturnsProductWithVariant_WhenThePassedInVariantHasMatchingParentProductId()
		{
			var productId = _randomizer.NextLong();
			var responseProducts = new List< Product > {
				new Product {
					Id = GraphQlIdsGenerator.CreateProductId( productId ),
					Title = _randomizer.GetString()
				}
			};
			var productVariant = new ProductVariant { Sku = _randomizer.GetString() };
			var variantsByProductId = new Dictionary< long, List< ProductVariant > > {
				{ productId, new List< ProductVariant > { productVariant } }
			};

			var result = responseProducts.ToShopifyProducts( variantsByProductId );

			Assert.Multiple(() => {
				Assert.That( result.Products.Single().Title, Is.EqualTo( responseProducts.Single().Title ) );
				Assert.That( result.Products.Single().Variants.Single().Sku, Is.EqualTo( productVariant.Sku ) );
			} );
		}

		[ Test ]
		public void ToShopifyProducts_ReturnsProductsWithoutVariants_WhenProductIdsDoNotMatchToVariantsParentProductIds()
		{
			var productId = _randomizer.NextLong( 10000 );
			var responseProducts = new List< Product > {
				new Product {
					Id = GraphQlIdsGenerator.CreateProductId( productId ),
					Title = _randomizer.GetString()
				}
			};
			var productVariant = new ProductVariant { Sku = _randomizer.GetString() };
			var differentProductId = productId + 1;
			var variantsByProductId = new Dictionary< long, List< ProductVariant > > {
				{ differentProductId, new List< ProductVariant > { productVariant } }
			};

			var result = responseProducts.ToShopifyProducts( variantsByProductId );

			Assert.Multiple(() => {
				Assert.That( result.Products.Single().Title, Is.EqualTo( responseProducts.Single().Title ) );
				Assert.That( result.Products.Single().Variants, Is.Empty );
			} );
		}
	}
}