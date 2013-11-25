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

		#region Misc
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
				var endpoint = EndpointsBuilder.CreateGetFirstProductsPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit, "variants" ) );

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
				var endpoint = EndpointsBuilder.CreateGetFirstProductsPageEndpoint( new ShopifyCommandEndpointConfig( i + 1, RequestMaxLimit, "variants" ) );

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
			var endpoint = EndpointsBuilder.CreateGetFirstProductsPageEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit, "variants" ) );

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
			var endpoint = EndpointsBuilder.CreateGetFirstProductsPageEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit, "variants" ) );

			await ActionPolicies.QueryAsync.Do( async () =>
				{
					products = await this._webRequestServices.GetResponseAsync< ShopifyProducts >( ShopifyCommand.GetProducts, endpoint );

					//API requirement
					Thread.Sleep( TimeSpan.FromSeconds( 0.6 ) );
				} );

			return products;
		}

		private int CalculatePagesCount( int productsCount )
		{
			var result = ( int )Math.Ceiling( ( double )productsCount / RequestMaxLimit );
			return result;
		}
		#endregion
	}
}