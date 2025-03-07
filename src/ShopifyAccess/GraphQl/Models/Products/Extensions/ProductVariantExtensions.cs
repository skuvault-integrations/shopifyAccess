using ShopifyAccess.GraphQl.Models.Common;
using ShopifyAccess.Misc;
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
				ImageUrl = productVariant.Image?.Url?.GetUrlWithoutQueryPart() ?? string.Empty,
				UpdatedAt = productVariant.UpdatedAt ?? default
			};
		}
	}
}