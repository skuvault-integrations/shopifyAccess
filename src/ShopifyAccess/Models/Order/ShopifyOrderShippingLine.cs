using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order
{
	[ DataContract ]
	public class ShopifyOrderShippingLine
	{
		[ DataMember( Name = "id" ) ]
		public string Id{ get; set; }

		[ DataMember( Name = "title" ) ]
		public string Title{ get; set; }

		[ DataMember( Name = "price" ) ]
		public decimal Price{ get; set; }

		[ DataMember( Name = "code" ) ]
		public string Code{ get; set; }

		[ DataMember( Name = "source" ) ]
		public string Source{ get; set; }
	}
}