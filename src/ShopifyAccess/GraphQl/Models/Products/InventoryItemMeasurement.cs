using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.Common;

namespace ShopifyAccess.GraphQl.Models.Products
{
	internal class InventoryItemMeasurement
	{
		[ DataMember( Name = "weight" ) ]
		public Weight Weight{ get; set; }
	}
}