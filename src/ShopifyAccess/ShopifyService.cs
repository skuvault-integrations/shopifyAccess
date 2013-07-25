using System;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using ShopifyAccess.Models.Core.Configuration.Command;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Services;

namespace ShopifyAccess
{
	public class ShopifyService : IShopifyService
	{
		private ShopifyCommandConfig _config;
		private readonly WebRequestServices _webRequestServices;
		private readonly EndpointsBuilder _endpointsBuilder;

		public ShopifyService( ShopifyCommandConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._config = config;
			this._webRequestServices = new WebRequestServices( config );
			this._endpointsBuilder = new EndpointsBuilder();
		}

		public ShopifyOrders GetOrders( DateTime dateFrom, DateTime dateTo )
		{
			var endpoint = this._endpointsBuilder.CreateOrdersDateRangeEndpoint( dateFrom, dateTo );
			return this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetAllOrders, endpoint );
		}

		public async Task< ShopifyOrders > GetOrdersAsync( DateTime dateFrom, DateTime dateTo )
		{
			var endpoint = this._endpointsBuilder.CreateOrdersDateRangeEndpoint( dateFrom, dateTo );
			return await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetAllOrders, endpoint );
		}

		public ShopifyOrders GetOrders( ShopifyOrderFulfillmentStatus status )
		{
			var endpoint = this._endpointsBuilder.CreateOrdersFulfillmentStatusEndpoint( status );
			return this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetAllOrders, endpoint );
		}

		public async Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderFulfillmentStatus status )
		{
			var endpoint = this._endpointsBuilder.CreateOrdersFulfillmentStatusEndpoint( status );
			return await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetAllOrders, endpoint );
		}
	}
}