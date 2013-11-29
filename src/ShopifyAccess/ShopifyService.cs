using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using ShopifyAccess.Misc;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.Product;
using ShopifyAccess.Models.ProductVariant;
using ShopifyAccess.Services;
using ServiceStack.Text;

namespace ShopifyAccess
{
	public class ShopifyService : IShopifyService
	{
		private readonly WebRequestServices _webRequestServices;
		private const int RequestMaxLimit = 250;

		public ShopifyService( ShopifyCommandConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._webRequestServices = new WebRequestServices( config );
		}

		#region GetOrders
		public ShopifyOrders GetOrders( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo )
		{
			ShopifyOrders orders;
			var endpoint = EndpointsBuilder.CreateOrdersEndpoint( status, dateFrom, dateTo );
			var ordersCount = this.GetOrdersCount( endpoint );

			if( ordersCount > RequestMaxLimit )
				orders = this.CollectOrdersFromSinglePage( endpoint );
			else
				orders = this.CollectOrdersFromAllPages( endpoint, ordersCount );

			return orders;
		}

		public async Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo )
		{
			ShopifyOrders orders;
			var endpoint = EndpointsBuilder.CreateOrdersEndpoint( status, dateFrom, dateTo );
			var ordersCount = await this.GetOrdersCountAsync( endpoint );

			if( ordersCount > RequestMaxLimit )
				orders = await this.CollectOrdersFromSinglePageAsync( endpoint );
			else
				orders = await this.CollectOrdersFromAllPagesAsync( endpoint, ordersCount );

			return orders;
		}

		private ShopifyOrders CollectOrdersFromAllPages( string mainEndpoint, int ordersCount )
		{
			var pagesCount = this.CalculatePagesCount( ordersCount );
			var orders = new ShopifyOrders();

			for( var i = 0; i < pagesCount; i++ )
			{
				var endpoint = mainEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) ) );

				ActionPolicies.ShopifySubmitPolicy.Do( () =>
					{
						var ordersWithinPage = this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetOrders, endpoint );
						orders.Orders.AddRange( ordersWithinPage.Orders );

						//API requirement
						Thread.Sleep( TimeSpan.FromSeconds( 0.6 ) );
					} );
			}

			return orders;
		}

		private async Task< ShopifyOrders > CollectOrdersFromAllPagesAsync( string mainEndpoint, int ordersCount )
		{
			var pagesCount = this.CalculatePagesCount( ordersCount );
			var orders = new ShopifyOrders();

			for( var i = 0; i < pagesCount; i++ )
			{
				var endpoint = mainEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) ) );

				await ActionPolicies.QueryAsync.Do( async () =>
					{
						var ordersWithinPage = await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetOrders, endpoint );
						orders.Orders.AddRange( ordersWithinPage.Orders );

						//API requirement
						Thread.Sleep( TimeSpan.FromSeconds( 0.6 ) );
					} );
			}

			return orders;
		}

		private ShopifyOrders CollectOrdersFromSinglePage( string mainEndpoint )
		{
			ShopifyOrders orders = null;
			var endpoint = mainEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetSinglePageEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit ) ) );

			ActionPolicies.ShopifySubmitPolicy.Do( () =>
				{
					orders = this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetOrders, endpoint );

					//API requirement
					Thread.Sleep( TimeSpan.FromSeconds( 0.6 ) );
				} );

			return orders;
		}

		private async Task< ShopifyOrders > CollectOrdersFromSinglePageAsync( string mainEndpoint )
		{
			ShopifyOrders orders = null;
			var endpoint = mainEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetSinglePageEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit ) ) );

			await ActionPolicies.QueryAsync.Do( async () =>
				{
					orders = await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetOrders, endpoint );

					//API requirement
					Thread.Sleep( TimeSpan.FromSeconds( 0.6 ) );
				} );

			return orders;
		}

		private int GetOrdersCount( string endpoint )
		{
			var count = 0;
			ActionPolicies.ShopifySubmitPolicy.Do( () =>
				{
					count = this._webRequestServices.GetResponse< OrdersCount >( ShopifyCommand.GetOrdersCount, endpoint ).Count;
				} );
			return count;
		}

		private async Task< int > GetOrdersCountAsync( string endpoint )
		{
			var count = 0;
			await ActionPolicies.QueryAsync.Do( async () =>
				{
					count = ( await this._webRequestServices.GetResponseAsync< OrdersCount >( ShopifyCommand.GetOrdersCount, endpoint ) ).Count;
				} );
			return count;
		}
		#endregion

		#region Products
		public ShopifyProducts GetProducts()
		{
			ShopifyProducts products;
			var productsCount = this.GetProductsCount();

			if( productsCount > RequestMaxLimit )
				products = this.CollectProductsFromAllPages( productsCount );
			else
				products = this.CollectProductsFromSinglePage();

			return products;
		}

		public async Task< ShopifyProducts > GetProductsAsync()
		{
			ShopifyProducts products;
			var productsCount = await this.GetProductsCountAsync();

			if( productsCount > RequestMaxLimit )
				products = await this.CollectProductsFromAllPagesAsync( productsCount );
			else
				products = await this.CollectProductsFromSinglePageAsync();

			return products;
		}

		private int GetProductsCount()
		{
			var count = 0;
			ActionPolicies.ShopifySubmitPolicy.Do( () =>
				{
					count = this._webRequestServices.GetResponse< ProductsCount >( ShopifyCommand.GetProductsCount, EndpointsBuilder.EmptyEndpoint ).Count;
				} );
			return count;
		}

		private async Task< int > GetProductsCountAsync()
		{
			var count = 0;
			await ActionPolicies.QueryAsync.Do( async () =>
				{
					count = ( await this._webRequestServices.GetResponseAsync< ProductsCount >( ShopifyCommand.GetProductsCount, EndpointsBuilder.EmptyEndpoint ) ).Count;
				} );
			return count;
		}

		private ShopifyProducts CollectProductsFromAllPages( int productsCount )
		{
			var pagesCount = this.CalculatePagesCount( productsCount );
			var products = new ShopifyProducts();

			for( var i = 0; i < pagesCount; i++ )
			{
				var endpoint = EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) );

				ActionPolicies.ShopifySubmitPolicy.Do( () =>
					{
						var productsWithinPage = this._webRequestServices.GetResponse< ShopifyProducts >( ShopifyCommand.GetProducts, endpoint );
						products.Products.AddRange( productsWithinPage.Products );

						//API requirement
						Thread.Sleep( TimeSpan.FromSeconds( 0.6 ) );
					} );
			}

			return products;
		}

		private async Task< ShopifyProducts > CollectProductsFromAllPagesAsync( int productsCount )
		{
			var pagesCount = this.CalculatePagesCount( productsCount );
			var products = new ShopifyProducts();

			for( var i = 0; i < pagesCount; i++ )
			{
				var endpoint = EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) );

				await ActionPolicies.QueryAsync.Do( async () =>
					{
						var productsWithinPage = await this._webRequestServices.GetResponseAsync< ShopifyProducts >( ShopifyCommand.GetProducts, endpoint );
						products.Products.AddRange( productsWithinPage.Products );

						//API requirement
						Thread.Sleep( TimeSpan.FromSeconds( 0.6 ) );
					} );
			}

			return products;
		}

		private ShopifyProducts CollectProductsFromSinglePage()
		{
			ShopifyProducts products = null;
			var endpoint = EndpointsBuilder.CreateGetSinglePageEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit ) );

			ActionPolicies.ShopifySubmitPolicy.Do( () =>
				{
					products = this._webRequestServices.GetResponse< ShopifyProducts >( ShopifyCommand.GetProducts, endpoint );

					//API requirement
					Thread.Sleep( TimeSpan.FromSeconds( 0.6 ) );
				} );

			return products;
		}

		private async Task< ShopifyProducts > CollectProductsFromSinglePageAsync()
		{
			ShopifyProducts products = null;
			var endpoint = EndpointsBuilder.CreateGetSinglePageEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit ) );

			await ActionPolicies.QueryAsync.Do( async () =>
				{
					products = await this._webRequestServices.GetResponseAsync< ShopifyProducts >( ShopifyCommand.GetProducts, endpoint );

					//API requirement
					Thread.Sleep( TimeSpan.FromSeconds( 0.6 ) );
				} );

			return products;
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

			//API requirement
			Thread.Sleep( TimeSpan.FromSeconds( 0.6 ) );
		}

		private async Task UpdateProductVariantQuantityAsync( ShopifyProductVariant variant )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			var jsonContent = new { variant }.ToJson();

			await this._webRequestServices.PutDataAsync( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent );

			//API requirement
			Thread.Sleep( TimeSpan.FromSeconds( 0.6 ) );
		}
		#endregion

		#region Misc
		private int CalculatePagesCount( int productsCount )
		{
			var result = ( int )Math.Ceiling( ( double )productsCount / RequestMaxLimit );
			return result;
		}
		#endregion
	}
}