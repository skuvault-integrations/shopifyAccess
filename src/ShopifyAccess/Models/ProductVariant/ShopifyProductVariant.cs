using System.Runtime.Serialization;

namespace ShopifyAccess.Models.ProductVariant
{
	[ DataContract ]
	public class ShopifyProductVariant
	{
		[ DataMember( Name = "id" ) ]
		public long Id { get; set; }

		[ DataMember( Name = "inventory_quantity" ) ]
		public int Quantity { get; set; }

		[ DataMember( Name = "inventory_management" ) ]
		public InventoryManagement InventoryManagement { get; set; }
	}

	public enum InventoryManagement
	{
		Undefined,
		Blank,
		Shopify
	}
}