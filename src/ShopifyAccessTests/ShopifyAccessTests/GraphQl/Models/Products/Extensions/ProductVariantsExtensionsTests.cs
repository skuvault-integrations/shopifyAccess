using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ShopifyAccess.GraphQl.Models.Products;
using ShopifyAccess.GraphQl.Models.Products.Extensions;
using ShopifyAccessTests.GraphQl.Helpers;

namespace ShopifyAccessTests.GraphQl.Models.Products.Extensions
{
	public class ProductVariantsExtensionsTests
	{
		private static readonly Randomizer _randomizer = new Randomizer();

		[ Test ]
		public void AppendVariants_AddsVariants_WhenEmptyProductVariants()
		{
			var productVariants = new Dictionary< long, List< ProductVariant > >();	//No variants initially
			var productId = _randomizer.NextLong();
			var appendVariants = new List< ProductVariantWithProductId > {
				new ProductVariantWithProductId {
					Product = new VariantParentProduct { Id = GraphQlIdsGenerator.CreateProductId( productId ) },
					Sku = _randomizer.GetString()
				}
			};

			productVariants.AppendVariants( appendVariants );

			Assert.Multiple( () => {
				Assert.That( productVariants.SelectMany( x => x.Value ).Count(), Is.EqualTo( appendVariants.Count ) );
				Assert.That( productVariants[ productId ].Single(), Is.EqualTo( appendVariants.Single() ) );
			} );
		}

		[ Test ]
		public void AppendVariants_AddsVariants_WhenTwoDifferentProducts()
		{
			var existingProductId = _randomizer.NextLong( 10000 );
			var existingSku = _randomizer.GetString();
			var existingProductVariant = new ProductVariant { Sku = existingSku };
			var productVariants = new Dictionary< long, List< ProductVariant > > {
				{ existingProductId, new List< ProductVariant > { existingProductVariant } }
			};
			var productId = existingProductId + 1;
			var appendVariants = new List< ProductVariantWithProductId > {
				new ProductVariantWithProductId {
					Product = new VariantParentProduct { Id = GraphQlIdsGenerator.CreateProductId( productId ) },
					Sku = existingSku + "1" 
				}
			};

			productVariants.AppendVariants( appendVariants );

			Assert.Multiple( () => {
				Assert.That( productVariants.SelectMany( x => x.Value ).Count(),
					Is.EqualTo( 1 /* existing */ + appendVariants.Count ) );
				Assert.That( productVariants[ existingProductId ].Single(), Is.EqualTo( existingProductVariant ) );
				Assert.That( productVariants[ productId ], Is.EquivalentTo(
					appendVariants.Select( x => ( ProductVariant )x ) ) );
			} );
		}

		[ Test ]
		public void AppendVariants_AddsVariants_andDoesNotAlterExistingVariant_WhenProductAlreadyHasVariants()
		{
			var productId = _randomizer.NextLong( 10000 );
			var existingSku = _randomizer.GetString();
			var existingProductVariant = new ProductVariant { Sku = existingSku };
			var productVariants = new Dictionary< long, List< ProductVariant > > {
				{ productId, new List< ProductVariant > { existingProductVariant } }
			};
			var appendVariants = new List< ProductVariantWithProductId > {
				new ProductVariantWithProductId {
					Product = new VariantParentProduct { Id = GraphQlIdsGenerator.CreateProductId( productId ) },
					Sku = existingSku + "1" 
				}
			};

			productVariants.AppendVariants( appendVariants );

			Assert.Multiple( () => {
				Assert.That( productVariants.SelectMany( x => x.Value ).Count(),
					Is.EqualTo( 1 /* existing */ + appendVariants.Count ) );
				Assert.That( productVariants, Has.Count.EqualTo( 1 ) );	// Just one product
				var product1Variants = productVariants[ productId ];
				Assert.That( product1Variants[ 0 ], Is.EqualTo( existingProductVariant ) );
				Assert.That( product1Variants[ 1 ], Is.EqualTo( appendVariants[ 0 ] ) );
			} );
		}

		[ Test ]
		//This is an abnormal case. It's not worth deduplicating SKUs since in the normal flow Shopify won't return duplicate product variant.
		//	And if they do, the sync will just process a SKU twice, in the order Shopify returned them
		public void AppendVariants_AddsDuplicateVariants_WhenProductAlreadyHasVariantWithSameSku()
		{
			var productId = _randomizer.NextLong( 10000 );
			var sku = _randomizer.GetString();
			var existingProductVariant = new ProductVariant { Sku = sku };
			var productVariants = new Dictionary< long, List< ProductVariant > > {
				{ productId, new List< ProductVariant > { existingProductVariant } }
			};
			var appendVariants = new List< ProductVariantWithProductId > {
				new ProductVariantWithProductId {
					Product = new VariantParentProduct { Id = GraphQlIdsGenerator.CreateProductId( productId ) },
					Sku = sku 
				}
			};

			productVariants.AppendVariants( appendVariants );

			Assert.Multiple( () => {
				Assert.That( productVariants.SelectMany( x => x.Value ).Count(),
					Is.EqualTo( 1 /* existing */ + appendVariants.Count ) );
				Assert.That( productVariants, Has.Count.EqualTo( 1 ) );	// Just one product
				var product1Variants = productVariants[ productId ];
				Assert.That( product1Variants[ 0 ], Is.EqualTo( existingProductVariant ) );
				Assert.That( product1Variants[ 1 ], Is.EqualTo( appendVariants[ 0 ] ) );
			} );
		}
	}
}