using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using ServiceStack;
using ShopifyAccess.Misc;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.Product;
using ShopifyAccess.Models.ProductVariant;
using ShopifyAccess.Services;

namespace ShopifyAccess
{
	public sealed class ShopifyService : IShopifyService
	{
		private readonly WebRequestServices _webRequestServices;
		private readonly TimeSpan DefaultApiDelay = TimeSpan.FromSeconds( 0.6 );
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
			var newOrdersEndpoint = EndpointsBuilder.CreateNewOrdersEndpoint( status, dateFrom, dateTo );
			var updatedOrdersEndpoint = EndpointsBuilder.CreateUpdatedOrdersEndpoint( status, dateFrom, dateTo );

			var ordersCount = this.GetOrdersCount( newOrdersEndpoint, updatedOrdersEndpoint );

			if( ordersCount > RequestMaxLimit )
				orders = this.CollectOrdersFromSinglePage( newOrdersEndpoint, updatedOrdersEndpoint );
			else
				orders = this.CollectOrdersFromAllPages( newOrdersEndpoint, updatedOrdersEndpoint, ordersCount );

			return orders;
		}

		public async Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo )
		{
			ShopifyOrders orders;
			var newOrdersEndpoint = EndpointsBuilder.CreateNewOrdersEndpoint( status, dateFrom, dateTo );
			var updatedOrdersEndpoint = EndpointsBuilder.CreateUpdatedOrdersEndpoint( status, dateFrom, dateTo );

			var ordersCount = await this.GetOrdersCountAsync( newOrdersEndpoint, updatedOrdersEndpoint );

			if( ordersCount > RequestMaxLimit )
				orders = await this.CollectOrdersFromSinglePageAsync( newOrdersEndpoint, updatedOrdersEndpoint );
			else
				orders = await this.CollectOrdersFromAllPagesAsync( newOrdersEndpoint, updatedOrdersEndpoint, ordersCount );

			return orders;
		}

		private ShopifyOrders CollectOrdersFromAllPages( string mainNewOrdersEndpoint, string mainUpdatedOrdersEndpoint, int ordersCount )
		{
			var pagesCount = this.CalculatePagesCount( ordersCount );
			var orders = new ShopifyOrders();

			for( var i = 0; i < pagesCount; i++ )
			{
				var compositeNewOrdersEndpoint = mainNewOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) ) );
				var compositeUpdatedOrdersEndpoint = mainUpdatedOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) ) );

				ActionPolicies.ShopifySubmitPolicy.Do( () =>
				{
					var newOrdersWithinPage = this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetOrders, compositeNewOrdersEndpoint );
					var updatedOrdersWithinPage = this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetOrders, compositeUpdatedOrdersEndpoint );
					var allOrders = newOrdersWithinPage.Orders.Concat( updatedOrdersWithinPage.Orders );

					orders.Orders.AddRange( allOrders );

					//API requirement
					this.CreateApiDelay().Wait();
				} );
			}

			return orders;
		}

		private async Task< ShopifyOrders > CollectOrdersFromAllPagesAsync( string mainNewOrdersEndpoint, string mainUpdatedOrdersEndpoint, int ordersCount )
		{
			var pagesCount = this.CalculatePagesCount( ordersCount );
			var orders = new ShopifyOrders();

			for( var i = 0; i < pagesCount; i++ )
			{
				var compositeNewOrdersEndpoint = mainNewOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) ) );
				var compositeUpdatedOrdersEndpoint = mainUpdatedOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) ) );

				await ActionPolicies.QueryAsync.Do( async () =>
				{
					var newOrdersWithinPage = await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetOrders, compositeNewOrdersEndpoint );
					var updatedOrdersWithinPage = await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetOrders, compositeUpdatedOrdersEndpoint );
					var allOrders = newOrdersWithinPage.Orders.Concat( updatedOrdersWithinPage.Orders );

					orders.Orders.AddRange( allOrders );

					//API requirement
					await this.CreateApiDelay();
				} );
			}

			return orders;
		}

		private ShopifyOrders CollectOrdersFromSinglePage( string mainNewOrdersEndpoint, string mainUpdatedOrdersEndpoint )
		{
			ShopifyOrders orders = null;
			var compositeNewOrdersEndpoint = mainNewOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetSinglePageEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit ) ) );
			var compositeUpdatedOrdersEndpoint = mainUpdatedOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetSinglePageEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit ) ) );

			ActionPolicies.ShopifySubmitPolicy.Do( () =>
			{
				var newOrders = this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetOrders, compositeNewOrdersEndpoint );
				var updatedOrders = this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetOrders, compositeUpdatedOrdersEndpoint );

				orders = new ShopifyOrders( newOrders.Orders.Concat( updatedOrders.Orders ).ToList() );

				//API requirement
				this.CreateApiDelay().Wait();
			} );

			return orders;
		}

		private async Task< ShopifyOrders > CollectOrdersFromSinglePageAsync( string mainNewOrdersEndpoint, string mainUpdatedOrdersEndpoint )
		{
			ShopifyOrders orders = null;
			var compositeNewOrdersEndpoint = mainNewOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetSinglePageEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit ) ) );
			var compositeUpdatedOrdersEndpoint = mainUpdatedOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetSinglePageEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit ) ) );

			await ActionPolicies.QueryAsync.Do( async () =>
			{
				var newOrders = await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetOrders, compositeNewOrdersEndpoint );
				var updatedOrders = await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetOrders, compositeUpdatedOrdersEndpoint );

				orders = new ShopifyOrders( newOrders.Orders.Concat( updatedOrders.Orders ).ToList() );

				//API requirement
				await this.CreateApiDelay();
			} );

			return orders;
		}

		private int GetOrdersCount( string newOrdersEndpoint, string updatedOrdersEndpoint )
		{
			var count = 0;
			ActionPolicies.ShopifySubmitPolicy.Do( () =>
			{
				var newOrdersCount = this._webRequestServices.GetResponse< OrdersCount >( ShopifyCommand.GetOrdersCount, newOrdersEndpoint ).Count;
				var updatedOrdersCount = this._webRequestServices.GetResponse< OrdersCount >( ShopifyCommand.GetOrdersCount, updatedOrdersEndpoint ).Count;

				count = newOrdersCount + updatedOrdersCount;
			} );
			return count;
		}

		private async Task< int > GetOrdersCountAsync( string newOrdersEndpoint, string updatedOrdersEndpoint )
		{
			var count = 0;
			await ActionPolicies.QueryAsync.Do( async () =>
			{
				var newOrdersCount = ( await this._webRequestServices.GetResponseAsync< OrdersCount >( ShopifyCommand.GetOrdersCount, newOrdersEndpoint ) ).Count;
				var updatedOrdersCount = ( await this._webRequestServices.GetResponseAsync< OrdersCount >( ShopifyCommand.GetOrdersCount, updatedOrdersEndpoint ) ).Count;

				count = newOrdersCount + updatedOrdersCount;
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

			this.RemoveUntrackedProductVariants( products );

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

			this.RemoveUntrackedProductVariants( products );

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
					this.CreateApiDelay().Wait();
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
					await this.CreateApiDelay();
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
				this.CreateApiDelay().Wait();
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
				await this.CreateApiDelay();
			} );

			return products;
		}

		private void RemoveUntrackedProductVariants( ShopifyProducts products )
		{
			foreach( var product in products.Products )
			{
				product.Variants.RemoveAll( v => v.InventoryManagement == InventoryManagement.Blank );
			}
		}
		#endregion

		#region Update variants
		public void UpdateProductVariants( IEnumerable< ShopifyProductVariant > variants )
		{
			foreach( var variant in variants )
			{
				ActionPolicies.ShopifySubmitPolicy.Do( () => this.UpdateProductVariantQuantity( variant ) );
			}
		}

		public async Task UpdateProductVariantsAsync( IEnumerable< ShopifyProductVariant > variants )
		{
			foreach( var variant in variants )
			{
				await ActionPolicies.QueryAsync.Do( async () => await this.UpdateProductVariantQuantityAsync( variant ) );
			}
		}

		private void UpdateProductVariantQuantity( ShopifyProductVariant variant )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			//just simpliest way to serialize with the root name.
			var jsonContent = new { variant }.ToJson();

			this._webRequestServices.PutData( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent );

			//API requirement
			this.CreateApiDelay().Wait();
		}

		private async Task UpdateProductVariantQuantityAsync( ShopifyProductVariant variant )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			var jsonContent = new { variant }.ToJson();

			await this._webRequestServices.PutDataAsync( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent );

			//API requirement
			await this.CreateApiDelay();
		}
		#endregion

		#region Misc
		private int CalculatePagesCount( int productsCount )
		{
			var result = ( int )Math.Ceiling( ( double )productsCount / RequestMaxLimit );
			return result;
		}

		private Task CreateApiDelay()
		{
			return Task.Delay( this.DefaultApiDelay );
		}
		#endregion
	}
}