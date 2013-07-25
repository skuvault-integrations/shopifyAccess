using ShopifyAccess.Models.Order;

namespace ShopifyAccess
{
	public interface IShopifyService
	{
		ShopifyOrders GetOrders();
	}
}