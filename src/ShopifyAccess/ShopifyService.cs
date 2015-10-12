using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using ServiceStack;
using ShopifyAccess.Misc;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Location;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.Product;
using ShopifyAccess.Models.ProductVariant;
using ShopifyAccess.Models.User;
using ShopifyAccess.Services;

namespace ShopifyAccess
{
	public sealed class ShopifyService: IShopifyService
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
			var updatedOrdersEndpoint = EndpointsBuilder.CreateUpdatedOrdersEndpoint( status, dateFrom, dateTo );

			var ordersCount = this.GetOrdersCount( updatedOrdersEndpoint );
			var orders = this.CollectOrdersFromAllPages( updatedOrdersEndpoint, ordersCount );
			return orders;
		}

		public async Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo )
		{
			var updatedOrdersEndpoint = EndpointsBuilder.CreateUpdatedOrdersEndpoint( status, dateFrom, dateTo );

			var ordersCount = await this.GetOrdersCountAsync( updatedOrdersEndpoint );
			var orders = await this.CollectOrdersFromAllPagesAsync( updatedOrdersEndpoint, ordersCount );
			return orders;
		}

		public ShopifyLocations GetLocations()
		{
			var locations = ActionPolicies.ShopifyGetPolicy.Get( () => this._webRequestServices.GetResponse< ShopifyLocations >( ShopifyCommand.GetLocations, "" ) );
			return locations;
		}

		public async Task< ShopifyLocations > GetLocationsAsync()
		{
			var locations = await ActionPolicies.ShopifyGetPolicyAsync.Get( async () => ( await this._webRequestServices.GetResponseAsync< ShopifyLocations >( ShopifyCommand.GetLocations, "" ) ) );
			return locations;
		}

		private ShopifyOrders CollectOrdersFromAllPages( string mainUpdatedOrdersEndpoint, int ordersCount )
		{
			var pagesCount = this.CalculatePagesCount( ordersCount );
			var orders = new ShopifyOrders();

			for( var i = 0; i < pagesCount; i++ )
			{
				var compositeUpdatedOrdersEndpoint = mainUpdatedOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) ) );

				ActionPolicies.ShopifyGetPolicy.Do( () =>
				{
					var updatedOrdersWithinPage = this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetOrders, compositeUpdatedOrdersEndpoint );
					orders.Orders.AddRange( updatedOrdersWithinPage.Orders );

					//API requirement
					this.CreateApiDelay().Wait();
				} );
			}

			return orders;
		}

		private async Task< ShopifyOrders > CollectOrdersFromAllPagesAsync( string mainUpdatedOrdersEndpoint, int ordersCount )
		{
			var pagesCount = this.CalculatePagesCount( ordersCount );
			var orders = new ShopifyOrders();

			for( var i = 0; i < pagesCount; i++ )
			{
				var compositeUpdatedOrdersEndpoint = mainUpdatedOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) ) );

				await ActionPolicies.ShopifyGetPolicyAsync.Do( async () =>
				{
					var updatedOrdersWithinPage = await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetOrders, compositeUpdatedOrdersEndpoint );
					orders.Orders.AddRange( updatedOrdersWithinPage.Orders );

					//API requirement
					await this.CreateApiDelay();
				} );
			}

			return orders;
		}

		private int GetOrdersCount( string updatedOrdersEndpoint )
		{
			var count = ActionPolicies.ShopifyGetPolicy.Get( () =>
			{
				var updatedOrdersCount = this._webRequestServices.GetResponse< OrdersCount >( ShopifyCommand.GetUsers, updatedOrdersEndpoint ).Count;
				return updatedOrdersCount;
			} );
			return count;
		}

		private async Task< int > GetOrdersCountAsync( string updatedOrdersEndpoint )
		{
			var count = await ActionPolicies.ShopifyGetPolicyAsync.Get( async () =>
			{
				var updatedOrdersCount = ( await this._webRequestServices.GetResponseAsync< OrdersCount >( ShopifyCommand.GetOrdersCount, updatedOrdersEndpoint ) ).Count;
				return updatedOrdersCount;
			} );
			return count;
		}
		#endregion

		#region Products
		public ShopifyProducts GetProducts()
		{
			var productsCount = this.GetProductsCount();
			var products = this.CollectProductsFromAllPages( productsCount );
			this.RemoveUntrackedProductVariants( products );

			return products;
		}

		public async Task< ShopifyProducts > GetProductsAsync()
		{
			var productsCount = await this.GetProductsCountAsync();
			var products = await this.CollectProductsFromAllPagesAsync( productsCount );
			this.RemoveUntrackedProductVariants( products );

			return products;
		}

		private int GetProductsCount()
		{
			var count = ActionPolicies.ShopifyGetPolicy.Get( () =>
				this._webRequestServices.GetResponse< ProductsCount >( ShopifyCommand.GetProductsCount, EndpointsBuilder.EmptyEndpoint ).Count );
			return count;
		}

		private async Task< int > GetProductsCountAsync()
		{
			var count = await ActionPolicies.ShopifyGetPolicyAsync.Get( async () =>
				( await this._webRequestServices.GetResponseAsync< ProductsCount >( ShopifyCommand.GetProductsCount, EndpointsBuilder.EmptyEndpoint ) ).Count );
			return count;
		}

		private ShopifyProducts CollectProductsFromAllPages( int productsCount )
		{
			var pagesCount = this.CalculatePagesCount( productsCount );
			var products = new ShopifyProducts();

			for( var i = 0; i < pagesCount; i++ )
			{
				var endpoint = EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) );

				ActionPolicies.ShopifyGetPolicy.Do( () =>
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

				await ActionPolicies.ShopifyGetPolicyAsync.Do( async () =>
				{
					var productsWithinPage = await this._webRequestServices.GetResponseAsync< ShopifyProducts >( ShopifyCommand.GetProducts, endpoint );
					products.Products.AddRange( productsWithinPage.Products );

					//API requirement
					await this.CreateApiDelay();
				} );
			}

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
		public void UpdateProductVariants( IEnumerable< ShopifyProductVariantForUpdate > variants )
		{
			foreach( var variant in variants )
			{
				ActionPolicies.ShopifySubmitPolicy.Do( () => this.UpdateProductVariantQuantity( variant ) );
			}
		}

		public async Task UpdateProductVariantsAsync( IEnumerable< ShopifyProductVariantForUpdate > variants )
		{
			foreach( var variant in variants )
			{
				await ActionPolicies.ShopifySubmitPolicyAsync.Do( async () => await this.UpdateProductVariantQuantityAsync( variant ) );
			}
		}

		private void UpdateProductVariantQuantity( ShopifyProductVariantForUpdate variant )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			//just simpliest way to serialize with the root name.
			var jsonContent = new { variant }.ToJson();

			this._webRequestServices.PutData( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent );

			//API requirement
			this.CreateApiDelay().Wait();
		}

		private async Task UpdateProductVariantQuantityAsync( ShopifyProductVariantForUpdate variant )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			var jsonContent = new { variant }.ToJson();

			await this._webRequestServices.PutDataAsync( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent );

			//API requirement
			await this.CreateApiDelay();
		}
		#endregion

		#region Users
		public ShopifyUsers GetUsers()
		{
			var users = ActionPolicies.ShopifyGetPolicy.Get( () => this._webRequestServices.GetResponse< ShopifyUsers >( ShopifyCommand.GetUsers, "" ) );
			return users;
		}

		public async Task< ShopifyUsers > GetUsersAsync()
		{
			var users = await ActionPolicies.ShopifyGetPolicyAsync.Get( async () => await this._webRequestServices.GetResponseAsync< ShopifyUsers >( ShopifyCommand.GetUsers, "" ) );
			return users;
		}

		public bool DoesShopifyPlusCustomer()
		{
			try
			{
				this._webRequestServices.GetResponse< ShopifyUsers >( ShopifyCommand.GetUsers, "" );
				return true;
			}
			catch( Exception )
			{
				return false;
			}
		}

		public async Task< bool > DoesShopifyPlusCustomerAsync()
		{
			try
			{
				await this._webRequestServices.GetResponseAsync< ShopifyUsers >( ShopifyCommand.GetUsers, "" );
				return true;
			}
			catch( Exception )
			{
				return false;
			}
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