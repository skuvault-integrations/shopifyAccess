using ShopifyAccess.GraphQl.Models.Common;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccess.GraphQl.Models.Products.Extensions
{
	internal static class ProductVariantExtensions
	{
		public static ShopifyProductVariant ToShopifyProductVariant( this ProductVariant productVariant )
		{
			if ( productVariant == null )
				return null;
			return new ShopifyProductVariant
			{
				Sku = productVariant.Sku,
				Title = productVariant.Title,
				Barcode = productVariant.Barcode,
				Weight = ( decimal )(productVariant.InventoryItem?.Measurement?.Weight?.Value ?? default),
				WeightUnit = productVariant.InventoryItem?.Measurement?.Weight?.UnitStandardized ?? WeightUnit.POUNDS,
				Price = productVariant.Price ?? default,
				//TODO GUARD-3717: Add image id, url, as needed
				UpdatedAt = productVariant.UpdatedAt ?? default
			};
		}
	}
}