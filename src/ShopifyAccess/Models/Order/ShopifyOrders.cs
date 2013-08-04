using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order
{
	[ DataContract ]
	public class ShopifyOrders
	{
		[ DataMember( Name = "orders" ) ]
		public IList< ShopifyOrder > Orders { get; private set; }

		public int Count
		{
			get { return this.Orders.Count; }
		}
	}
}