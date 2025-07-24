using System.Collections.Generic;
using ShopifyAccess.Models.Order;

namespace ShopifyAccess.GraphQl.Models.Orders.Extensions
{
	public static class OrderExtensions
	{
		internal static ShopifyOrder ToShopifyOrder( this Order order )
		{
			return new ShopifyOrder
			{
				Id = order.Id,
			};
		}
	}
}
