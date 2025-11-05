using System.Collections.Generic;
using System.Linq;
using ShopifyAccess.GraphQl.Models.Common.Extensions;
using ShopifyAccess.Models.Product;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccess.GraphQl.Models.Products.Extensions
{
	public static class ProductExtensions
	{
		//TODO GUARD-3954 Remove on feature cleanup
		internal static ShopifyProduct ToShopifyProductLegacy( this Product product )
		{
			var productVariants = product.Variants?.Items ?? new List< ProductVariant >();
			return new ShopifyProduct
			{
				Title = product.Title,
				Vendor = product.Vendor,
				Images = product.Media?.Items?.ToShopifyProductImages() ?? new List< ShopifyAccess.Models.ShopifyProductImage >(),
				Variants = productVariants.Select( x => x.ToShopifyProductVariant() ).ToList(),
				Type = product.ProductType,
				BodyHtml = product.DescriptionHtml,
				UpdatedAt = product.UpdatedAt ?? default
			};
		}

		internal static ShopifyProduct ToShopifyProduct( this Product product, List< ProductVariant > productVariants = null )
		{
			return new ShopifyProduct
			{
				Title = product.Title,
				Vendor = product.Vendor,
				Images = product.Media?.Items?.ToShopifyProductImages() ?? new List< ShopifyAccess.Models.ShopifyProductImage >(),
				Variants = productVariants?
					.Select( x => x.ToShopifyProductVariant() ).ToList() ?? new List< ShopifyProductVariant >(),
				Type = product.ProductType,
				BodyHtml = product.DescriptionHtml,
				UpdatedAt = product.UpdatedAt ?? default
			};
		}
	}
}