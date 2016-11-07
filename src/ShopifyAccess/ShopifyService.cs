using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using ServiceStack;
using ShopifyAccess.Misc;
using ShopifyAccess.Models;
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
		private readonly string _shopName;

		public ShopifyService( ShopifyCommandConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._webRequestServices = new WebRequestServices( config );
			this._shopName = config.ShopName;
		}

		#region GetOrders
		public ShopifyOrders GetOrders( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo, Mark mark = null )
		{
			var updatedOrdersEndpoint = EndpointsBuilder.CreateUpdatedOrdersEndpoint( status, dateFrom, dateTo );

			var ordersCount = this.GetOrdersCount( updatedOrdersEndpoint, mark.CreateNewIfBlank() );
			var orders = this.CollectOrdersFromAllPages( updatedOrdersEndpoint, ordersCount, mark.CreateNewIfBlank() );
			return orders;
		}

		public async Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo, Mark mark = null )
		{
			var updatedOrdersEndpoint = EndpointsBuilder.CreateUpdatedOrdersEndpoint( status, dateFrom, dateTo );

			var ordersCount = await this.GetOrdersCountAsync( updatedOrdersEndpoint, mark.CreateNewIfBlank() );
			var orders = await this.CollectOrdersFromAllPagesAsync( updatedOrdersEndpoint, ordersCount, mark.CreateNewIfBlank() );
			return orders;
		}

		public ShopifyLocations GetLocations( Mark mark = null )
		{
			var locations = ActionPolicies.CreateShopifyGetPolicy( mark.CreateNewIfBlank(), this._shopName ).Get( () => this._webRequestServices.GetResponse< ShopifyLocations >( ShopifyCommand.GetLocations, "", mark.CreateNewIfBlank() ) );
			return locations;
		}

		public async Task< ShopifyLocations > GetLocationsAsync( Mark mark = null )
		{
			var locations = await ActionPolicies.CreateShopifyGetPolicyAsync( mark.CreateNewIfBlank(), this._shopName ).Get( async () => ( await this._webRequestServices.GetResponseAsync< ShopifyLocations >( ShopifyCommand.GetLocations, "", mark.CreateNewIfBlank() ) ) );
			return locations;
		}

		private ShopifyOrders CollectOrdersFromAllPages( string mainUpdatedOrdersEndpoint, int ordersCount, Mark mark = null )
		{
			var pagesCount = this.CalculatePagesCount( ordersCount );
			var orders = new ShopifyOrders();

			for( var i = 0; i < pagesCount; i++ )
			{
				var compositeUpdatedOrdersEndpoint = mainUpdatedOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) ) );

				ActionPolicies.CreateShopifyGetPolicy( mark.CreateNewIfBlank(), this._shopName ).Do( () =>
				{
					var updatedOrdersWithinPage = this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetOrders, compositeUpdatedOrdersEndpoint, mark.CreateNewIfBlank() );
					orders.Orders.AddRange( updatedOrdersWithinPage.Orders );

					//API requirement
					this.CreateApiDelay().Wait();
				} );
			}

			return orders;
		}

		private async Task< ShopifyOrders > CollectOrdersFromAllPagesAsync( string mainUpdatedOrdersEndpoint, int ordersCount, Mark mark = null )
		{
			var pagesCount = this.CalculatePagesCount( ordersCount );
			var orders = new ShopifyOrders();

			for( var i = 0; i < pagesCount; i++ )
			{
				var compositeUpdatedOrdersEndpoint = mainUpdatedOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) ) );

				await ActionPolicies.CreateShopifyGetPolicyAsync( mark.CreateNewIfBlank(), this._shopName ).Do( async () =>
				{
					var updatedOrdersWithinPage = await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetOrders, compositeUpdatedOrdersEndpoint, mark.CreateNewIfBlank() );
					orders.Orders.AddRange( updatedOrdersWithinPage.Orders );

					//API requirement
					await this.CreateApiDelay();
				} );
			}

			return orders;
		}

		private int GetOrdersCount( string updatedOrdersEndpoint, Mark mark = null )
		{
			var count = ActionPolicies.CreateShopifyGetPolicy( mark.CreateNewIfBlank(), this._shopName ).Get( () =>
			{
				var updatedOrdersCount = this._webRequestServices.GetResponse< OrdersCount >( ShopifyCommand.GetOrdersCount, updatedOrdersEndpoint, mark.CreateNewIfBlank() ).Count;
				return updatedOrdersCount;
			} );
			return count;
		}

		private async Task< int > GetOrdersCountAsync( string updatedOrdersEndpoint, Mark mark = null )
		{
			var count = await ActionPolicies.CreateShopifyGetPolicyAsync( mark.CreateNewIfBlank(), this._shopName ).Get( async () =>
			{
				var updatedOrdersCount = ( await this._webRequestServices.GetResponseAsync< OrdersCount >( ShopifyCommand.GetOrdersCount, updatedOrdersEndpoint, mark.CreateNewIfBlank() ) ).Count;
				return updatedOrdersCount;
			} );
			return count;
		}
		#endregion

		#region Products
		public ShopifyProducts GetProducts( Mark mark = null )
		{
			var productsCount = this.GetProductsCount( mark.CreateNewIfBlank() );
			var products = this.CollectProductsFromAllPages( productsCount, mark.CreateNewIfBlank() );
			this.RemoveUntrackedProductVariants( products );

			return products;
		}

		public async Task< ShopifyProducts > GetProductsAsync( Mark mark = null )
		{
			var productsCount = await this.GetProductsCountAsync( mark.CreateNewIfBlank() );
			var products = await this.CollectProductsFromAllPagesAsync( productsCount, mark );
			this.RemoveUntrackedProductVariants( products );

			return products;
		}

		private int GetProductsCount( Mark mark = null )
		{
			var count = ActionPolicies.CreateShopifyGetPolicy( mark.CreateNewIfBlank(), this._shopName ).Get( () =>
				this._webRequestServices.GetResponse< ProductsCount >( ShopifyCommand.GetProductsCount, EndpointsBuilder.EmptyEndpoint, mark.CreateNewIfBlank() ).Count );
			return count;
		}

		private async Task< int > GetProductsCountAsync( Mark mark = null )
		{
			var count = await ActionPolicies.CreateShopifyGetPolicyAsync( mark.CreateNewIfBlank(), this._shopName ).Get( async () =>
					( await this._webRequestServices.GetResponseAsync< ProductsCount >( ShopifyCommand.GetProductsCount, EndpointsBuilder.EmptyEndpoint, mark.CreateNewIfBlank() ) ).Count );
			return count;
		}

		private ShopifyProducts CollectProductsFromAllPages( int productsCount, Mark mark = null )
		{
			var pagesCount = this.CalculatePagesCount( productsCount );
			var products = new ShopifyProducts();

			for( var i = 0; i < pagesCount; i++ )
			{
				var endpoint = EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) );

				ActionPolicies.CreateShopifyGetPolicy( mark.CreateNewIfBlank(), this._shopName ).Do( () =>
				{
					var productsWithinPage = this._webRequestServices.GetResponse< ShopifyProducts >( ShopifyCommand.GetProducts, endpoint, mark.CreateNewIfBlank() );
					products.Products.AddRange( productsWithinPage.Products );

					//API requirement
					this.CreateApiDelay().Wait();
				} );
			}

			return products;
		}

		private async Task< ShopifyProducts > CollectProductsFromAllPagesAsync( int productsCount, Mark mark )
		{
			var pagesCount = this.CalculatePagesCount( productsCount );
			var products = new ShopifyProducts();

			for( var i = 0; i < pagesCount; i++ )
			{
				var endpoint = EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) );

				await ActionPolicies.CreateShopifyGetPolicyAsync( mark.CreateNewIfBlank(), this._shopName ).Do( async () =>
				{
					var productsWithinPage = await this._webRequestServices.GetResponseAsync< ShopifyProducts >( ShopifyCommand.GetProducts, endpoint, mark );
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
		public void UpdateProductVariants( IEnumerable< ShopifyProductVariantForUpdate > variants, Mark mark = null )
		{
			foreach( var variant in variants )
			{
				ActionPolicies.CreateSubmitPolicy( mark.CreateNewIfBlank(), this._shopName ).Do( () => this.UpdateProductVariantQuantity( variant, mark.CreateNewIfBlank() ) );
			}
		}

		public async Task UpdateProductVariantsAsync( IEnumerable< ShopifyProductVariantForUpdate > variants, Mark mark = null )
		{
			foreach( var variant in variants )
			{
				await ActionPolicies.CreateShopifySubmitPolicyAsync( mark.CreateNewIfBlank(), this._shopName ).Do( async () => await this.UpdateProductVariantQuantityAsync( variant, mark.CreateNewIfBlank() ) );
			}
		}

		private void UpdateProductVariantQuantity( ShopifyProductVariantForUpdate variant, Mark mark = null )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			//just simpliest way to serialize with the root name.
			var jsonContent = new { variant }.ToJson();

			this._webRequestServices.PutData( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent, mark.CreateNewIfBlank() );

			//API requirement
			this.CreateApiDelay().Wait();
		}

		private async Task UpdateProductVariantQuantityAsync( ShopifyProductVariantForUpdate variant, Mark mark = null )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			var jsonContent = new { variant }.ToJson();

			await this._webRequestServices.PutDataAsync( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent, mark.CreateNewIfBlank() );

			//API requirement
			await this.CreateApiDelay();
		}
		#endregion

		#region Users
		public ShopifyUsers GetUsers( Mark mark = null )
		{
			var users = ActionPolicies.CreateShopifyGetPolicy( mark.CreateNewIfBlank(), this._shopName ).Get( () => this._webRequestServices.GetResponse< ShopifyUsers >( ShopifyCommand.GetUsers, "", mark.CreateNewIfBlank() ) );
			return users;
		}

		public async Task< ShopifyUsers > GetUsersAsync( Mark mark = null )
		{
			var users = await ActionPolicies.CreateShopifyGetPolicyAsync( mark.CreateNewIfBlank(), this._shopName ).Get( async () => await this._webRequestServices.GetResponseAsync< ShopifyUsers >( ShopifyCommand.GetUsers, "", mark.CreateNewIfBlank() ) );
			return users;
		}

		public ShopifyUser GetUser( long id, Mark mark = null )
		{
			var user = ActionPolicies.CreateShopifyGetPolicy( mark.CreateNewIfBlank(), this._shopName ).Get( () => this._webRequestServices.GetResponse< ShopifyUserWrapper >( ShopifyCommand.GetUser, EndpointsBuilder.CreateGetUserEndpoint( id ), mark.CreateNewIfBlank() ) );
			return user.User;
		}

		public async Task< ShopifyUser > GetUserAsync( long id, Mark mark = null )
		{
			var user = await ActionPolicies.CreateShopifyGetPolicyAsync( mark.CreateNewIfBlank(), this._shopName ).Get( async () => await this._webRequestServices.GetResponseAsync< ShopifyUserWrapper >( ShopifyCommand.GetUser, EndpointsBuilder.CreateGetUserEndpoint( id ), mark.CreateNewIfBlank() ) );
			return user.User;
		}

		public bool DoesShopifyPlusAccount( Mark mark = null )
		{
			try
			{
				this._webRequestServices.GetResponse< ShopifyUsers >( ShopifyCommand.GetUsers, "", mark.CreateNewIfBlank() );
				return true;
			}
			catch( Exception )
			{
				return false;
			}
		}

		public async Task< bool > DoesShopifyPlusAccountAsync( Mark mark = null )
		{
			try
			{
				await this._webRequestServices.GetResponseAsync< ShopifyUsers >( ShopifyCommand.GetUsers, "", mark.CreateNewIfBlank() );
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