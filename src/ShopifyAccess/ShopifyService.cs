using System;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using ShopifyAccess.Models.Core.Configuration.Command;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.ProductVariant;
using ShopifyAccess.Services;
using ServiceStack.Text;

namespace ShopifyAccess
{
	public class ShopifyService : IShopifyService
	{
		private readonly WebRequestServices _webRequestServices;

		public ShopifyService( ShopifyCommandConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._webRequestServices = new WebRequestServices( config );
		}

		#region GetOrders
		public ShopifyOrders GetOrders( DateTime dateFrom, DateTime dateTo )
		{
			var endpoint = EndpointsBuilder.CreateOrdersEndpoint( dateFrom, dateTo );
			return this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetAllOrders, endpoint );
		}

		public async Task< ShopifyOrders > GetOrdersAsync( DateTime dateFrom, DateTime dateTo )
		{
			var endpoint = EndpointsBuilder.CreateOrdersEndpoint( dateFrom, dateTo );
			return await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetAllOrders, endpoint );
		}

		public ShopifyOrders GetOrders( ShopifyOrderFulfillmentStatus status, DateTime dateFrom, DateTime dateTo )
		{
			var endpoint = EndpointsBuilder.CreateOrdersEndpoint( status, dateFrom, dateTo );
			return this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetAllOrders, endpoint );
		}

		public async Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderFulfillmentStatus status, DateTime dateFrom, DateTime dateTo )
		{
			var endpoint = EndpointsBuilder.CreateOrdersEndpoint( status, dateFrom, dateTo );
			return await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetAllOrders, endpoint );
		}
		#endregion

		#region Update variants
		public void UpdateProductVariantQuantity( ProductVariant variant )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			//just simpliest way to serialize with the root name.
			var jsonContent = new { variant }.ToJson();

			this._webRequestServices.PutData( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent );
		}

		public async Task UpdateProductVariantQuantityAsync( ProductVariant variant )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			var jsonContent = new { variant }.ToJson();

			await this._webRequestServices.PutDataAsync( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent );
		}
		#endregion
	}
}