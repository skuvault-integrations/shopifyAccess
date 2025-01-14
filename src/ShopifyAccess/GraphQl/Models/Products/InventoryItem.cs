using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Products
{
	internal class InventoryItem
	{
		[ DataMember( Name = "measurement" ) ]
		public InventoryItemMeasurement Measurement{ get; set; }
	}
}