using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using ShopifyAccess.Misc;
using ShopifyAccess.Models.Configuration.Command;
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
			ShopifyOrders orders = null;

			ActionPolicies.ShopifySubmitPolicy.Do( () =>
				{
					var endpoint = EndpointsBuilder.CreateOrdersEndpoint( dateFrom, dateTo );
					orders = this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetAllOrders, endpoint );
				} );

			return orders;
		}

		public async Task< ShopifyOrders > GetOrdersAsync( DateTime dateFrom, DateTime dateTo )
		{
			ShopifyOrders orders = null;

			await ActionPolicies.QueryAsync.Do( async () =>
				{
					var endpoint = EndpointsBuilder.CreateOrdersEndpoint( dateFrom, dateTo );
					orders = await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetAllOrders, endpoint );
				} );

			return orders;
		}

		public ShopifyOrders GetOrders( ShopifyOrderFulfillmentStatus status, DateTime dateFrom, DateTime dateTo )
		{
			ShopifyOrders orders = null;

			ActionPolicies.ShopifySubmitPolicy.Do( () =>
				{
					var endpoint = EndpointsBuilder.CreateOrdersEndpoint( status, dateFrom, dateTo );
					orders = this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetAllOrders, endpoint );
				} );

			return orders;
		}

		public async Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderFulfillmentStatus status, DateTime dateFrom, DateTime dateTo )
		{
			ShopifyOrders orders = null;

			await ActionPolicies.QueryAsync.Do( async () =>
				{
					var endpoint = EndpointsBuilder.CreateOrdersEndpoint( status, dateFrom, dateTo );
					orders = await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetAllOrders, endpoint );
				} );

			return orders;
		}
		#endregion

		#region Update variants
		public void UpdateProductVariants( IEnumerable< ShopifyProductVariant > variants )
		{
			foreach( var variant in variants )
				ActionPolicies.ShopifySubmitPolicy.Do( () => this.UpdateProductVariantQuantity( variant ) );
		}

		public async Task UpdateProductVariantsAsync( IEnumerable< ShopifyProductVariant > variants )
		{
			foreach( var variant in variants )
				await ActionPolicies.QueryAsync.Do( async () => await this.UpdateProductVariantQuantityAsync( variant ) );
		}

		private void UpdateProductVariantQuantity( ShopifyProductVariant variant )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			//just simpliest way to serialize with the root name.
			var jsonContent = new { variant }.ToJson();

			this._webRequestServices.PutData( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent );
		}

		private async Task UpdateProductVariantQuantityAsync( ShopifyProductVariant variant )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			var jsonContent = new { variant }.ToJson();

			await this._webRequestServices.PutDataAsync( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent );
		}
		#endregion
	}
}