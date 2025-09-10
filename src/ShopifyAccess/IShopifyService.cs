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
		// TODO GUARD-3910 Remove this legacy method and switch to the GraphQL version.
		Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo, CancellationToken token, Mark mark = null );
		
		/// <summary>
		/// Get shipped orders async
		/// </summary>
		/// <param name="dateFrom">Show orders created after date (format: 2008-01-01 03:00)</param>
		/// <param name="dateTo">Show orders created before date (format: 2008-01-01 03:00)</param>
		/// <param name="status">Shipped,partial, unshipped or any</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		// TODO GUARD-3910 Remove the 'V2' suffix, it was only used to distinguish from the legacy REST version.
		Task< ShopifyOrders > GetOrdersV2Async( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo, CancellationToken token, Mark mark = null );

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
		/// Get all product variants for inventory (through locations)
		/// </summary>
		/// <param name="token">CancellationToken</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns>Product variants (inventory items)</returns>
		Task< List< ShopifyProductVariant > > GetProductVariantsInventoryAsync( CancellationToken token, Mark mark );

		/// <summary>
		/// Get the list of product variants for specified SKUs
		/// </summary>
		/// <param name="skus">List of SKU for search</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns>List of variants (inventory items)</returns>
		Task< List< ShopifyProductVariant > > GetProductVariantsInventoryBySkusAsync( IEnumerable< string > skus, CancellationToken token, Mark mark );

		/// <summary>
		/// Get all existing product variants with inventory levels
		/// This method uses GraphQl API to get data
		/// </summary>
		/// <param name="token">CancellationToken</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns>Product variants with inventory levels</returns>
		Task< List< ShopifyProductVariant > > GetProductVariantsInventoryReportAsync( CancellationToken token, Mark mark );

		/// <summary>
		/// Get the list of product variants for specified SKUs
		/// This method uses GraphQl API to get data
		/// </summary>
		/// <param name="skus">List of SKU for search</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns>List of variants (inventory items)</returns>
		Task< List< ShopifyProductVariant > > GetProductVariantsInventoryReportBySkusAsync( IEnumerable< string > skus, CancellationToken token, Mark mark );

		/// <summary>
		///     Update inventory levels (inventory items) async
		/// </summary>
		/// <param name="token">Cancellation tokens</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <param name="inventoryLevels"></param>
		Task UpdateInventoryLevelsAsync( IEnumerable< ShopifyInventoryLevelForUpdate > inventoryLevels, CancellationToken token, Mark mark );

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