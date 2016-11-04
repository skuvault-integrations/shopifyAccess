using System;
using System.Collections.Generic;
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
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns>Orders collection</returns>
		ShopifyOrders GetOrders( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo, Mark mark = null );

		/// <summary>
		/// Get shipped orders async
		/// </summary>
		/// <param name="dateFrom">created_at_min. Show orders created after date (format: 2008-01-01 03:00)</param>
		/// <param name="dateTo">created_at_max. Show orders created before date (format: 2008-01-01 03:00)</param>
		/// <param name="status">fulfillment_status. Shipped,partial, unshipped or any</param>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo, Mark mark = null );

		/// <summary>
		/// get locations
		/// </summary>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		ShopifyLocations GetLocations( Mark mark = null );

		/// <summary>
		/// get locations async
		/// </summary>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		Task< ShopifyLocations > GetLocationsAsync( Mark mark = null );

		/// <summary>
		/// Get all existing products
		/// </summary>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns>Products with variants (inventory items)</returns>
		ShopifyProducts GetProducts( Mark mark = null );

		/// <summary>
		/// Get all existing products async
		/// </summary>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns>Products with variants (inventory items)</returns>
		Task< ShopifyProducts > GetProductsAsync( Mark mark = null );

		/// <summary>
		/// Update variants (inventory items)
		/// </summary>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		void UpdateProductVariants( IEnumerable< ShopifyProductVariantForUpdate > variants, Mark mark = null );

		/// <summary>
		/// Update variants (inventory items) async
		/// </summary>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		Task UpdateProductVariantsAsync( IEnumerable< ShopifyProductVariantForUpdate > variants, Mark mark = null );

		/// <summary>
		/// Get all users
		/// </summary>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		ShopifyUsers GetUsers( Mark mark = null );

		/// <summary>
		/// Get all users async
		/// </summary>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		Task< ShopifyUsers > GetUsersAsync( Mark mark = null );

		/// <summary>
		/// Get user
		/// </summary>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		ShopifyUser GetUser( long id, Mark mark = null );

		/// <summary>
		/// Get user async
		/// </summary>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		Task< ShopifyUser > GetUserAsync( long id, Mark mark = null );

		/// <summary>
		/// Does shopify plus customer
		/// </summary>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		bool DoesShopifyPlusAccount( Mark mark = null );

		/// <summary>
		/// Does shopify plus customer async
		/// </summary>
		/// <param name="mark">Mark is a special tag, which help to search in logs</param>
		/// <returns></returns>
		Task< bool > DoesShopifyPlusAccountAsync( Mark mark = null );
	}
}