using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order
{
	[ DataContract ]
	public class ShopifyOrder
	{
		[ DataMember( Name = "id" ) ]
		public string Id { get; set; }

		[ DataMember( Name = "total_price" ) ]
		public string Total { get; set; }

		[ DataMember( Name = "line_items" ) ]
		public IList< ShopifyOrderItem > OrderItems { get; set; }
	}
}