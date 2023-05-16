using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.ProductVariantsInventory
{
	[ DataContract ]
	internal class InventoryLevelsNodes
	{
		[ DataMember( Name = "nodes" ) ]
		public List< InventoryLevel > Nodes{ get; set; } = new List< InventoryLevel >();
	}
}