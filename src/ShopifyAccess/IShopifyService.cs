using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShopifyAccess.Models;
using ShopifyAccess.Models.Location;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.Product;
using ShopifyAccess.Models.ProductVariant;
using ShopifyAccess.Models.User;

namespace ShopifyAccess
{
	public interface IShopifyService
	{
		/// <summary>
		/// Get orders by date and fulfillment status
		/// </summary>
		/// <param name="status">Fulfillment_status. Shipped, partial, unshipped or any</param>
		/// <param name="dateFrom">created_at_min. Show orders created after date (format: 2008-01-01 03:00)</param>
		/// <param name="dateTo">created_at_max. Show orders created before date (format: 2008-01-01 03:00)</param>
		/// <param name="token"></param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns>Orders collection</returns>
		ShopifyOrders GetOrders( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo, CancellationToken token, Mark mark = null );

		/// <summary>
		/// Get shipped orders async
		/// </summary>
		/// <param name="dateFrom">created_at_min. Show orders created after date (format: 2008-01-01 03:00)</param>
		/// <param name="dateTo">created_at_max. Show orders created before date (format: 2008-01-01 03:00)</param>
		/// <param name="status">fulfillment_status. Shipped,partial, unshipped or any</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo, CancellationToken token, Mark mark = null );

		/// <summary>
		/// get locations
		/// </summary>
		/// <param name="token"></param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		ShopifyLocations GetLocations( CancellationToken token, Mark mark = null );

		/// <summary>
		/// Get all Shopify locations for the shop
		/// </summary>
		/// <param name="token">CancellationToken</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		Task< ShopifyLocations > GetLocationsAsync( CancellationToken token, Mark mark = null );

		/// <summary>
		/// Get active Shopify locations for the shop
		/// </summary>
		/// <param name="token">CancellationToken</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		Task< ShopifyLocations > GetActiveLocationsAsync( CancellationToken token, Mark mark = null );

		/// <summary>
		/// Get products created after the given date async
		/// </summary>
		/// <param name="productsStartUtc"></param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>Products with variants (inventory items)</returns>
		Task< ShopifyProducts > GetProductsCreatedAfterAsync( DateTime productsStartUtc, CancellationToken token, Mark mark );

		/// <summary>
		/// Get products created before the given date but updated after async
		/// </summary>
		/// <param name="productsStartUtc"></param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>Products with variants (inventory items)</returns>
		Task< ShopifyProducts > GetProductsCreatedBeforeButUpdatedAfterAsync( DateTime productsStartUtc, CancellationToken token, Mark mark );

		/// <summary>
		/// Get all existing products async (through locations)
		/// </summary>
		/// <param name="token">CancellationToken</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns>Products with variants (inventory items)</returns>
		Task< ShopifyProducts > GetProductsInventoryAsync( CancellationToken token, Mark mark = null );

		/// <summary>
		/// Get the list of product variants for specified SKUs
		/// </summary>
		/// <param name="skus">List of SKU for search</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns>List of variants (inventory items)</returns>
		Task< List< ShopifyProductVariant > > GetProductVariantsInventoryBySkusAsync( IEnumerable< string > skus, CancellationToken token, Mark mark = null );

		/// <summary>
		/// Get all existing product variants with inventory levels
		/// This method uses GraphQl API to get data
		/// </summary>
		/// <param name="token">CancellationToken</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns>Product variants with inventory levels</returns>
		Task< List< ShopifyProductVariant > > GetProductVariantsInventoryReportAsync( CancellationToken token, Mark mark = null );

		/// <summary>
		/// Get the list of product variants for specified SKUs
		/// This method uses GraphQl API to get data
		/// </summary>
		/// <param name="skus">List of SKU for search</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns>List of variants (inventory items)</returns>
		Task< List< ShopifyProductVariant > > GetProductVariantsInventoryReportBySkusAsync( IEnumerable< string > skus, CancellationToken token, Mark mark = null );

		/// <summary>
		///     Update inventory levels (inventory items)
		/// </summary>
		/// <param name="token">Cancellation tokens</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <param name="inventoryLevels"></param>
		void UpdateInventoryLevels( IEnumerable< ShopifyInventoryLevelForUpdate > inventoryLevels, CancellationToken token, Mark mark = null );

		/// <summary>
		///     Update inventory levels (inventory items) async
		/// </summary>
		/// <param name="token">Cancellation tokens</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <param name="inventoryLevels"></param>
		Task UpdateInventoryLevelsAsync( IEnumerable< ShopifyInventoryLevelForUpdate > inventoryLevels, CancellationToken token, Mark mark = null );

		/// <summary>
		/// Get all users
		/// </summary>
		/// <param name="token"></param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		ShopifyUsers GetUsers( CancellationToken token, Mark mark = null );

		/// <summary>
		/// Get all users async
		/// </summary>
		/// <param name="token">CancellationToken</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		Task< ShopifyUsers > GetUsersAsync( CancellationToken token, Mark mark = null );

		/// <summary>
		/// Is Shopify Plus customer
		/// </summary>
		/// <param name="token"></param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		bool IsShopifyPlusAccount( CancellationToken token, Mark mark = null );

		/// <summary>
		/// Is Shopify Plus customer async
		/// </summary>
		/// <param name="token">CancellationToken</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		Task< bool > IsShopifyPlusAccountAsync( CancellationToken token, Mark mark = null );

		/// <summary>
		///	This property can be used by the client to monitor the last access library's network activity time.
		/// </summary>
		DateTime LastActivityTime { get; }
	}
}