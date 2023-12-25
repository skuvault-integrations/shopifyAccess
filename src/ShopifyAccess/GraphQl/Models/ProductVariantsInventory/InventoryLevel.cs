using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.ProductVariantsInventory
{
	[ DataContract ]
	internal class InventoryLevel
	{
		[ DataMember( Name = "quantities") ]
		public List< InventoryQuantity > Quantities { get; set; } = new List< InventoryQuantity >();

		[ DataMember( Name = "location" ) ]
		public Location Location{ get; set; }

		[ DataMember( Name = "__parentId" ) ]
		public string ProductVariantId{ get; set; }
	}
}