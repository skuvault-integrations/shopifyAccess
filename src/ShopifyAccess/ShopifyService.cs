using CuttingEdge.Conditions;
using ShopifyAccess.Models.Core;
using ShopifyAccess.Models.Core.Configuration;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Services;

namespace ShopifyAccess
{
	public class ShopifyService : IShopifyService
	{
		private ShopifyCommandConfig _config;
		private readonly WebRequestServices _webRequestServices;

		public ShopifyService( ShopifyCommandConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._config = config;
			this._webRequestServices = new WebRequestServices( config );
		}

		public ShopifyOrders GetOrders()
		{
			return this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetAllOrders );
		}
	}
}