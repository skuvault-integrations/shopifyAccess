using System;
using System.Runtime.Serialization;

namespace ShopifyAccess.Models.ProductVariant
{
	[ DataContract ]
	public class ShopifyInventoryLevelForUpdate
	{
		[ DataMember( Name = "inventory_item_id") ]
		public long InventoryItemId{ get; set; }

		[ DataMember( Name = "location_id" ) ]
		public long LocationId{ get; set; }

		[ DataMember( Name = "available") ]
		public int Quantity{ get; set; }
	}

	[ DataContract ]
	public class ShopifyInventoryLevelForUpdateResponse
	{
		[ DataMember( Name = "inventory_item_id" ) ]
		public long InventoryItemId{ get; set; }

		[ DataMember( Name = "location_id" ) ]
		public long LocationId{ get; set; }

		[ DataMember( Name = "available" ) ]
		public int Quantity{ get; set; }

		[ DataMember( Name = "updated_at" ) ]
		public string UpdateAt{ get; set; }
	}
}