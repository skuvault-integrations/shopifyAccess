using System.Collections.Generic;
using System.Linq;
using ShopifyAccess.GraphQl.Models.Common.Extensions;
using ShopifyAccess.Models.Product;

namespace ShopifyAccess.GraphQl.Models.Products.Extensions
{
	public static class ProductExtensions
	{
		//TODO GUARD-3717: Add tests
		internal static ShopifyProduct ToShopifyProduct( this Product product )
		{
			var productVariants = product.Variants?.Items ?? new List< ProductVariant >();
			return new ShopifyProduct
			{
				Title = product.Title,
				Vendor = product.Vendor,
				Images = product.Media?.Items?.ToShopifyProductImages(),
				Variants = productVariants.Select( x => x.ToShopifyProductVariant() ).ToList(),
				Type = product.ProductType,
				BodyHtml = product.DescriptionHtml,
				UpdatedAt = product.UpdatedAt ?? default
			};
		}
	}
}