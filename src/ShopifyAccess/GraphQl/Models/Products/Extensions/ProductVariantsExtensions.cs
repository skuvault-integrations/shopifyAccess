using System.Collections.Generic;
using System.Linq;
using ShopifyAccess.GraphQl.Helpers;
using ShopifyAccess.GraphQl.Models.Common;
using ShopifyAccess.Misc;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccess.GraphQl.Models.Products.Extensions
{
	internal static class ProductVariantsExtensions
	{
		/// <summary>
		/// Append <paramref name="productVariantToAdd" /> to <paramref name="existingProductVariants" />
		/// </summary>
		/// <param name="existingProductVariants">Dictionary of productVariants keyed off the parent productId</param>
		/// <param name="productVariantToAdd"></param>
		internal static void AppendVariants( this IDictionary< long, List< ProductVariant > > existingProductVariants, List< ProductVariantWithProductId > productVariantToAdd )
		{
			if( existingProductVariants == null || ( !productVariantToAdd?.Any() ?? true ) )
			{
				return;
			}

			foreach( var productVariant in productVariantToAdd )
			{
				var productId = GraphQlIdParser.Product.GetId( productVariant.Product.Id );
				if( existingProductVariants.ContainsKey( productId ) )
				{
					existingProductVariants[ productId ].Add( productVariant );
				}
				else
				{
					existingProductVariants.Add( productId, new List< ProductVariant > { productVariant } );
				}
			}
		}
	}
}