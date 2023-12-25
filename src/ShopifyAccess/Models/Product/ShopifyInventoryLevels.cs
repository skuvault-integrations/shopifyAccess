using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Product
{
	[ DataContract ]
	public class ShopifyInventoryLevels
	{
		[ DataMember( Name = "inventory_levels" ) ]
		public List< ShopifyInventoryLevel > InventoryLevels{ get; set; }

		public ShopifyInventoryLevels()
		{
			this.InventoryLevels = new List< ShopifyInventoryLevel >();
		}
	}

	[ DataContract ]
	public class ShopifyInventoryLevel
	{
		[ DataMember( Name = "inventory_item_id" ) ]
		public long InventoryItemId{ get; set; }

		[ DataMember( Name = "location_id" ) ]
		public long LocationId{ get; set; }

		/// <summary>
		/// The available quantity of an inventory item at the inventory level's associated location.
		/// It returns 'null' in a case if the inventory item is not tracked https://shopify.dev/docs/api/admin-rest/2023-01/resources/inventorylevel#resource-object
		/// </summary>
		[ DataMember( Name = "available" ) ]
		public int? Available{ get; set; }

		[ DataMember( Name = "updated_at" ) ]
		public string UpdatedAt{ get; set; }
	}
}