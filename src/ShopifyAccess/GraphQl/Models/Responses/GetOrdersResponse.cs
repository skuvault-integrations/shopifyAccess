using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.Orders;
using ShopifyAccess.GraphQl.Models.Orders.Extensions;
using ShopifyAccess.Models.Order;

namespace ShopifyAccess.GraphQl.Models.Responses
{
	[ DataContract ]
	internal class GetOrdersResponse: GraphQlResponseWithPages< GetOrdersData, Order >
	{
		public override Nodes< Order > GetItemsAndPagingInfo()
		{
			return this.Data.Orders;
		}
	}

	[ DataContract ]
	internal class GetOrdersData
	{
		[ DataMember( Name = "orders" ) ]
		public Nodes< Order > Orders{ get; set; }
	}

	internal static class GetOrdersResponseExtensions
	{
		internal static ShopifyOrders ToShopifyOrders( this List< Order > responseOrders )
		{
			return responseOrders != null
				? new ShopifyOrders
					{ Orders = responseOrders.Select( x => x.ToShopifyOrder() ).ToList() }
				: new ShopifyOrders();
		}
	}
}
