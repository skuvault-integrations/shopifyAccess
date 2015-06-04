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
}