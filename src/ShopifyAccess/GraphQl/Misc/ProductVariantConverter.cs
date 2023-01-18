using System.Collections.Generic;
using System.Linq;
using ShopifyAccess.GraphQl.Models.ProductVariantsWithInventoryLevelsReport;
using ShopifyAccess.Models.Product;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccess.GraphQl.Misc
{
	internal static class ProductVariantConverter
	{
		public static ShopifyProductVariant ToShopifyProductVariant( this ProductVariant variant )
		{
			var inventoryItemId = GraphQlIdParser.InventoryItem.GetId( variant.InventoryItem?.InventoryItemId );
			return new ShopifyProductVariant()
			{
				Id = GraphQlIdParser.ProductVariant.GetId( variant.ProductVariantId ),
				Sku = variant.Sku,
				InventoryItemId = inventoryItemId,
				InventoryLevels = GetShopifyInventoryLevels( variant.InventoryLevels, inventoryItemId )
			};
		}

		private static ShopifyInventoryLevels GetShopifyInventoryLevels( IEnumerable< InventoryLevel > inventories, long inventoryItemId )
		{
			var result = new ShopifyInventoryLevels();
			result.InventoryLevels.AddRange( inventories.Select( inv => new ShopifyInventoryLevel()
			{
				InventoryItemId = inventoryItemId,
				Available = inv.Available,
				LocationId = GraphQlIdParser.Location.GetId( inv.Location?.LocationId )
			} ) );
			return result;
		}
	}
}