using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.ProductVariantsInventoryReport
{
	[ DataContract ]
	internal class InventoryLevel
	{
		[ DataMember( Name = "available" ) ]
		public int Available{ get; set; }

		[ DataMember( Name = "location" ) ]
		public Location Location{ get; set; }

		[ DataMember( Name = "__parentId" ) ]
		public string ProductVariantId{ get; set; }
	}
}