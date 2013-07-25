using System;
using System.Threading.Tasks;
using ShopifyAccess.Models.Core.Configuration.Command;
using ShopifyAccess.Models.Order;

namespace ShopifyAccess
{
	public interface IShopifyService
	{
		ShopifyOrders GetOrders( DateTime dateFrom, DateTime dateTo );
		Task< ShopifyOrders > GetOrdersAsync( DateTime dateFrom, DateTime dateTo );
		ShopifyOrders GetOrders( ShopifyOrderFulfillmentStatus status );
		Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderFulfillmentStatus status );
	}
}