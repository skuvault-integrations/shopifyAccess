using System;
using System.Threading.Tasks;
using ShopifyAccess.Models.Core.Configuration.Command;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccess
{
	public interface IShopifyService
	{
		/// <summary>
		/// Get orders by date range
		/// </summary>
		/// <param name="dateFrom">created_at_min. Show orders created after date (format: 2008-01-01 03:00)</param>
		/// <param name="dateTo">created_at_max. Show orders created before date (format: 2008-01-01 03:00)</param>
		/// <returns>Orders collection</returns>
		ShopifyOrders GetOrders( DateTime dateFrom, DateTime dateTo );

		/// <summary>
		/// Get orders by date range async
		/// </summary>
		/// <param name="dateFrom">created_at_min. Show orders created after date (format: 2008-01-01 03:00)</param>
		/// <param name="dateTo">created_at_max. Show orders created before date (format: 2008-01-01 03:00)</param>
		/// <returns>Orders collection</returns>
		Task< ShopifyOrders > GetOrdersAsync( DateTime dateFrom, DateTime dateTo );

		/// <summary>
		/// Get orders by date and fulfillment status
		/// </summary>
		/// <param name="status">Fulfillment_status. Shipped, partial, unshipped or any</param>
		/// <param name="dateFrom">created_at_min. Show orders created after date (format: 2008-01-01 03:00)</param>
		/// <param name="dateTo">created_at_max. Show orders created before date (format: 2008-01-01 03:00)</param>
		/// <returns>Orders collection</returns>
		ShopifyOrders GetOrders( ShopifyOrderFulfillmentStatus status, DateTime dateFrom, DateTime dateTo );

		/// <summary>
		/// Get shipped orders async
		/// </summary>
		/// <param name="dateFrom">created_at_min. Show orders created after date (format: 2008-01-01 03:00)</param>
		/// <param name="dateTo">created_at_max. Show orders created before date (format: 2008-01-01 03:00)</param>
		/// <param name="status">fulfillment_status. Shipped,partial, unshipped or any</param>
		Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderFulfillmentStatus status, DateTime dateFrom, DateTime dateTo );

		/// <summary>
		/// Updates variant (inventory item)
		/// </summary>
		void UpdateProductVariantQuantity( ShopifyProductVariant variant );

		/// <summary>
		/// Updates variant (inventory item) async
		/// </summary>
		Task UpdateProductVariantQuantityAsync( ShopifyProductVariant variant );
	}
}