using System.Runtime.Serialization;
using ShopifyAccess.Models.Order;

namespace ShopifyAccess.GraphQl.Models.Orders
{
	[ DataContract ]
	internal class ShippingLine
	{
		[ DataMember( Name = "id" ) ]
		public string Id{ get; set; }

		[ DataMember( Name = "code" ) ]
		public string Code{ get; set; }

		[ DataMember( Name = "source" ) ]
		public string Source{ get; set; }

		[ DataMember( Name = "title" ) ]
		public string Title{ get; set; }

		[ DataMember( Name = "originalPriceSet" ) ]
		public ShopifyPriceSet OriginalPriceSet{ get; set; }
	}
}
