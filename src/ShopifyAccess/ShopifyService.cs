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
			mark = mark.CreateNewIfBlank();
			var updatedOrdersEndpoint = EndpointsBuilder.CreateUpdatedOrdersEndpoint( status, dateFrom, dateTo );

			var ordersCount = this.GetOrdersCount( updatedOrdersEndpoint, mark );
			var orders = this.CollectOrdersFromAllPages( updatedOrdersEndpoint, ordersCount, mark );
			return orders;
		}

		public async Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var updatedOrdersEndpoint = EndpointsBuilder.CreateUpdatedOrdersEndpoint( status, dateFrom, dateTo );

			var ordersCount = await this.GetOrdersCountAsync( updatedOrdersEndpoint, mark );
			var orders = await this.CollectOrdersFromAllPagesAsync( updatedOrdersEndpoint, ordersCount, mark );
			return orders;
		}

		public ShopifyLocations GetLocations( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var locations = ActionPolicies.GetPolicy( mark, this._shopName ).Get(
				() => this._webRequestServices.GetResponse< ShopifyLocations >( ShopifyCommand.GetLocations, "", mark ) );
			return locations;
		}

		public async Task< ShopifyLocations > GetLocationsAsync( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var locations = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
				async () => ( await this._webRequestServices.GetResponseAsync< ShopifyLocations >( ShopifyCommand.GetLocations, "", mark ) ) );
			return locations;
		}

		private ShopifyOrders CollectOrdersFromAllPages( string mainUpdatedOrdersEndpoint, int ordersCount, Mark mark )
		{
			var pagesCount = this.CalculatePagesCount( ordersCount );
			var orders = new ShopifyOrders();

			for( var i = 0; i < pagesCount; i++ )
			{
				var compositeUpdatedOrdersEndpoint = mainUpdatedOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) ) );

				ActionPolicies.GetPolicy( mark, this._shopName ).Do( () =>
				{
					var updatedOrdersWithinPage = this._webRequestServices.GetResponse< ShopifyOrders >( ShopifyCommand.GetOrders, compositeUpdatedOrdersEndpoint, mark );
					orders.Orders.AddRange( updatedOrdersWithinPage.Orders );

					//API requirement
					this.CreateApiDelay().Wait();
				} );
			}

			return orders;
		}

		private async Task< ShopifyOrders > CollectOrdersFromAllPagesAsync( string mainUpdatedOrdersEndpoint, int ordersCount, Mark mark )
		{
			var pagesCount = this.CalculatePagesCount( ordersCount );
			var orders = new ShopifyOrders();

			for( var i = 0; i < pagesCount; i++ )
			{
				var compositeUpdatedOrdersEndpoint = mainUpdatedOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) ) );

				await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Do( async () =>
				{
					var updatedOrdersWithinPage = await this._webRequestServices.GetResponseAsync< ShopifyOrders >( ShopifyCommand.GetOrders, compositeUpdatedOrdersEndpoint, mark );
					orders.Orders.AddRange( updatedOrdersWithinPage.Orders );

					//API requirement
					await this.CreateApiDelay();
				} );
			}

			return orders;
		}

		private int GetOrdersCount( string updatedOrdersEndpoint, Mark mark )
		{
			var count = ActionPolicies.GetPolicy( mark, this._shopName ).Get( () =>
			{
				var updatedOrdersCount = this._webRequestServices.GetResponse< OrdersCount >( ShopifyCommand.GetOrdersCount, updatedOrdersEndpoint, mark ).Count;
				return updatedOrdersCount;
			} );
			return count;
		}

		private async Task< int > GetOrdersCountAsync( string updatedOrdersEndpoint, Mark mark )
		{
			var count = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get( async () =>
			{
				var updatedOrdersCount = ( await this._webRequestServices.GetResponseAsync< OrdersCount >( ShopifyCommand.GetOrdersCount, updatedOrdersEndpoint, mark ) ).Count;
				return updatedOrdersCount;
			} );
			return count;
		}
		#endregion

		#region Products
		public ShopifyProducts GetProducts( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();

			var productsCount = this.GetProductsCount( mark );
			var products = this.CollectProductsFromAllPages( productsCount, mark );
			this.RemoveUntrackedProductVariants( products );

			return products;
		}

		public async Task< ShopifyProducts > GetProductsAsync( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();

			var productsCount = await this.GetProductsCountAsync( mark );
			var products = await this.CollectProductsFromAllPagesAsync( productsCount, mark );
			this.RemoveUntrackedProductVariants( products );

			return products;
		}

		private int GetProductsCount( Mark mark )
		{
			var count = ActionPolicies.GetPolicy( mark, this._shopName ).Get( () =>
					this._webRequestServices.GetResponse< ProductsCount >( ShopifyCommand.GetProductsCount, EndpointsBuilder.EmptyEndpoint, mark ).Count );
			return count;
		}

		private async Task< int > GetProductsCountAsync( Mark mark )
		{
			var count = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get( async () =>
					( await this._webRequestServices.GetResponseAsync< ProductsCount >( ShopifyCommand.GetProductsCount, EndpointsBuilder.EmptyEndpoint, mark ) ).Count );
			return count;
		}

		private ShopifyProducts CollectProductsFromAllPages( int productsCount, Mark mark )
		{
			var pagesCount = this.CalculatePagesCount( productsCount );
			var products = new ShopifyProducts();

			for( var i = 0; i < pagesCount; i++ )
			{
				var endpoint = EndpointsBuilder.CreateGetNextPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit ) );

				ActionPolicies.GetPolicy( mark, this._shopName ).Do( () =>
				{
					var productsWithinPage = this._webRequestServices.GetResponse< ShopifyProducts >( ShopifyCommand.GetProducts, endpoint, mark );
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

				await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Do( async () =>
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
			mark = mark.CreateNewIfBlank();
			foreach( var variant in variants )
			{
				ActionPolicies.SubmitPolicy( mark, this._shopName ).Do( () => this.UpdateProductVariantQuantity( variant, mark ) );
			}
		}

		public async Task UpdateProductVariantsAsync( IEnumerable< ShopifyProductVariantForUpdate > variants, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			foreach( var variant in variants )
			{
				await ActionPolicies.SubmitPolicyAsync( mark, this._shopName ).Do( async () => await this.UpdateProductVariantQuantityAsync( variant, mark ) );
			}
		}

		private void UpdateProductVariantQuantity( ShopifyProductVariantForUpdate variant, Mark mark )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			//just simpliest way to serialize with the root name.
			var jsonContent = new { variant }.ToJson();

			this._webRequestServices.PutData( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent, mark );

			//API requirement
			this.CreateApiDelay().Wait();
		}

		private async Task UpdateProductVariantQuantityAsync( ShopifyProductVariantForUpdate variant, Mark mark )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			var jsonContent = new { variant }.ToJson();

			await this._webRequestServices.PutDataAsync( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent, mark );

			//API requirement
			await this.CreateApiDelay();
		}
		#endregion

		#region Users
		public ShopifyUsers GetUsers( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var users = ActionPolicies.GetPolicy( mark, this._shopName ).Get(
				() => this._webRequestServices.GetResponse< ShopifyUsers >( ShopifyCommand.GetUsers, "", mark ) );
			return users;
		}

		public async Task< ShopifyUsers > GetUsersAsync( Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var users = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
				async () => await this._webRequestServices.GetResponseAsync< ShopifyUsers >( ShopifyCommand.GetUsers, "", mark ) );
			return users;
		}

		public ShopifyUser GetUser( long id, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var user = ActionPolicies.GetPolicy( mark, this._shopName ).Get(
				() => this._webRequestServices.GetResponse< ShopifyUserWrapper >( ShopifyCommand.GetUser, EndpointsBuilder.CreateGetUserEndpoint( id ), mark ) );
			return user.User;
		}

		public async Task< ShopifyUser > GetUserAsync( long id, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var user = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
				async () => await this._webRequestServices.GetResponseAsync< ShopifyUserWrapper >( ShopifyCommand.GetUser, EndpointsBuilder.CreateGetUserEndpoint( id ), mark ) );
			return user.User;
		}

		public bool DoesShopifyPlusAccount( Mark mark = null )
		{
			try
			{
				mark = mark.CreateNewIfBlank();
				this._webRequestServices.GetResponse< ShopifyUsers >( ShopifyCommand.GetUsers, "", mark );
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
				mark = mark.CreateNewIfBlank();
				await this._webRequestServices.GetResponseAsync< ShopifyUsers >( ShopifyCommand.GetUsers, "", mark );
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