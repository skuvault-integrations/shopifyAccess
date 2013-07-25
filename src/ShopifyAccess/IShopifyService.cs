using System;
using System.Threading.Tasks;
using ShopifyAccess.Models.Core.Configuration.Command;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccess
{
	public interface IShopifyService
	{
		ShopifyOrders GetOrders( DateTime dateFrom, DateTime dateTo );
		Task< ShopifyOrders > GetOrdersAsync( DateTime dateFrom, DateTime dateTo );
		ShopifyOrders GetOrders( ShopifyOrderFulfillmentStatus status );
		Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderFulfillmentStatus status );

		ProductVariant UpdateProductVariantQuantity( ProductVariant variant );
		Task< ProductVariant > UpdateProductVariantQuantityAsync( ProductVariant variant );
	}
}