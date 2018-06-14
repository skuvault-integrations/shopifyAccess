using System.Runtime.Serialization;

namespace ShopifyAccess.Models.ProductVariant
{
	[ DataContract ]
	public class ShopifyProductVariantForUpdate
	{
		[ DataMember( Name = "id" ) ]
		public long Id{ get; set; }

		[ DataMember( Name = "inventory_quantity" ) ]
		public int Quantity{ get; set; }

		[ DataMember( Name = "old_inventory_quantity" ) ]
		public int OldQuantity{ get; set; }
	}

	public static class ShopifyProductVariantForUpdateExtensions
	{
		public static ShopifyInventoryLevelForUpdate ToShopifyProductVariantForUpdate( this ShopifyProductVariantForUpdate obj )
		{
			return new ShopifyInventoryLevelForUpdate
			{
				InventoryItemId = obj.Id,
				Quantity = obj.Quantity
			};
		}
	}
}