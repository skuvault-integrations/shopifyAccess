using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ShopifyAccess.GraphQl.Models.Products;
using ShopifyAccess.GraphQl.Models.Products.Extensions;

namespace ShopifyAccessTests.GraphQl.Models.Products.Extensions
{
	public class ProductVariantsExtensionsTests
	{
		private static readonly Randomizer _randomizer = new Randomizer();

		[ Test ]
		public void AppendVariants_AddsVariants_WhenNoneExist()
		{
			var existingVariants = new Dictionary< long, List< ProductVariant > >();
			var productId = _randomizer.NextLong();
			var sku =  _randomizer.GetString();
			var appendVariants = new List< ProductVariantWithProductId >
			{
				new ProductVariantWithProductId
				{
					Product = new VariantParentProduct { Id = CreateGraphQlProductId( productId ) },
					Sku = sku
				}
			};

			existingVariants.AppendVariants( appendVariants );

			Assert.Multiple( () => {
				Assert.That( existingVariants.Count, Is.EqualTo( appendVariants.Count ) );
				//TODO GUARD-3946 Assert key and value
			} );
		}

		//TODO GUARD-3946 AppendVariants_AddsVariants_andDoesNotAlterExistingVariant_WhenProductAlreadyHasVariants()

		//This is an abnormal case. It's not worth deduplicated since in the normal flow Shopify won't return duplicates. And if they do, the sync will just process a SKU twice
		//TODO GUARD-3946 Potentially, add AppendVariants_AddsDuplicateVariants_WhenProductAlreadyHasVariantWithSameSku()
		
		private static string CreateGraphQlProductId( long productId )
		{
			return $"gid://shopify/Product/{productId}";
		}
	}
}