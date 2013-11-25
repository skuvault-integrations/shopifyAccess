using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order
{
	[ DataContract ]
	public class OrdersCount
	{
		[ DataMember( Name = "count" ) ]
		public int Count { get; set; }
	}
}