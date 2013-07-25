using System.Threading.Tasks;
using ShopifyAccess.Models.Order;

namespace ShopifyAccess
{
	public interface IShopifyService
	{
		ShopifyOrders GetOrders();
		Task< ShopifyOrders > GetOrdersAsync();
	}
}