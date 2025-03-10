﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Netco.Extensions;
using ServiceStack;
using ShopifyAccess.GraphQl;
using ShopifyAccess.GraphQl.Models.Products;
using ShopifyAccess.GraphQl.Models.ProductVariantsInventory.Extensions;
using ShopifyAccess.GraphQl.Models.Responses;
using ShopifyAccess.GraphQl.Queries;
using ShopifyAccess.GraphQl.Services;
using ShopifyAccess.Misc;
using ShopifyAccess.Models;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Location;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.Product;
using ShopifyAccess.Models.ProductVariant;
using ShopifyAccess.Models.User;
using ShopifyAccess.Services;
using ProductVariant = ShopifyAccess.GraphQl.Models.ProductVariantsInventory.ProductVariant;

namespace ShopifyAccess
{
	public sealed class ShopifyService: IShopifyService
	{
		private readonly WebRequestServices _webRequestServices;
		private readonly IReportGenerator _reportGenerator;
		private const int RequestMaxLimit = 250;
		private const int RequestInventoryLevelsMaxLimit = 50;
		private readonly string _shopName;
		// One throttler for all requests because used same limit for all API
		private readonly ShopifyThrottler _throttler = new ShopifyThrottler();
		private readonly ShopifyThrottlerAsync _throttlerAsync = new ShopifyThrottlerAsync();
		private readonly ShopifyGraphQlThrottler _graphQlThrottler;

		// Separate throttler for updating to save limit for other syncs
		private readonly ShopifyThrottlerAsync _productUpdateThrottlerAsync = new ShopifyThrottlerAsync( 30 );
		private readonly ShopifyTimeouts _timeouts;
		private readonly ShopifyCommandFactory _shopifyCommandFactory;
		private readonly PaginationService _graphQlPaginationService;

		/// <summary>
		///	Last service's network activity time. Can be used to monitor service's state.
		/// </summary>
		public DateTime LastActivityTime
		{
			get
			{
				return this._webRequestServices.LastNetworkActivityTime ?? DateTime.UtcNow;
			}
		}

		public ShopifyService( ShopifyClientCredentials clientCredentials, ShopifyTimeouts operationsTimeouts, ShopifyCommandFactory shopifyCommandFactory )
		{
			Condition.Requires( clientCredentials, "clientCredentials" ).IsNotNull();
			Condition.Requires( operationsTimeouts, "operationsTimeouts" ).IsNotNull();
			Condition.Requires( shopifyCommandFactory, "shopifyCommandFactory" ).IsNotNull();

			this._shopName = clientCredentials.ShopName;
			this._webRequestServices = new WebRequestServices( clientCredentials );
			this._shopifyCommandFactory = shopifyCommandFactory;
			this._reportGenerator = new ReportGenerator( clientCredentials.ShopName, this._webRequestServices, this._shopifyCommandFactory );
			this._graphQlThrottler = new ShopifyGraphQlThrottler( clientCredentials.ShopName );
			this._graphQlPaginationService = new PaginationService( this._graphQlThrottler, this._shopName ); 
			this._timeouts = operationsTimeouts;
		}

		#region GetOrders
		public ShopifyOrders GetOrders( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo, CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();

			var updatedOrdersEndpoint = EndpointsBuilder.CreateUpdatedOrdersEndpoint( status, dateFrom, dateTo );
			var orders = this.CollectOrdersFromAllPages( updatedOrdersEndpoint, mark, token, this._timeouts[ ShopifyOperationEnum.GetOrders ] );

			return orders;
		}

		public async Task< ShopifyOrders > GetOrdersAsync( ShopifyOrderStatus status, DateTime dateFrom, DateTime dateTo, CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();

			var updatedOrdersEndpoint = EndpointsBuilder.CreateUpdatedOrdersEndpoint( status, dateFrom, dateTo );
			var orders = await this.CollectOrdersFromAllPagesAsync( updatedOrdersEndpoint, mark, token, this._timeouts[ ShopifyOperationEnum.GetOrders ] );

			return orders;
		}

		public ShopifyLocations GetLocations( CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var locations = ActionPolicies.GetPolicy( mark, this._shopName, token ).Get(
				() => this._throttler.Execute(
					() => this._webRequestServices.GetResponse< ShopifyLocations >( _shopifyCommandFactory.CreateGetLocationsCommand(), "", token, mark, this._timeouts[ ShopifyOperationEnum.GetLocations ] ) ) );
			return locations;
		}

		public Task< ShopifyLocations > GetLocationsAsync( CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var locations = ActionPolicies.GetPolicyAsync( mark, this._shopName, token ).Get(
				() => this._throttlerAsync.ExecuteAsync(
					() => this._webRequestServices.GetResponseAsync< ShopifyLocations >( _shopifyCommandFactory.CreateGetLocationsCommand(), "", token, mark, this._timeouts[ ShopifyOperationEnum.GetLocations ] ) ) );
			return locations;
		}

		public async Task< ShopifyLocations > GetActiveLocationsAsync( CancellationToken token, Mark mark = null )
		{
			var allLocations = await this.GetLocationsAsync( token, mark ).ConfigureAwait( false );
			var activeLocations = allLocations?.Locations?.Where( x => x.IsActive ) ?? new ShopifyLocation[] {};
			return new ShopifyLocations( activeLocations.ToList() );
		}

		private ShopifyOrders CollectOrdersFromAllPages( string mainUpdatedOrdersEndpoint, Mark mark, CancellationToken token, int timeout )
		{
			var orders = new ShopifyOrders();
			var compositeUpdatedOrdersEndpoint = mainUpdatedOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit ) ) );

			do
			{
				var updatedOrdersWithinPage = ActionPolicies.GetPolicy( mark, this._shopName, token ).Get(
					() => this._throttler.Execute(
						() => this._webRequestServices.GetResponsePage< ShopifyOrders >( _shopifyCommandFactory.CreateGetOrdersCommand(), 
							compositeUpdatedOrdersEndpoint, token, mark, timeout ) ) );

				if( updatedOrdersWithinPage.Response.Orders.Count == 0 )
					break;

				orders.Orders.AddRange( updatedOrdersWithinPage.Response.Orders );

				compositeUpdatedOrdersEndpoint = updatedOrdersWithinPage.NextPageQueryStr;
			} while( compositeUpdatedOrdersEndpoint != string.Empty );

			return orders;
		}

		private async Task< ShopifyOrders > CollectOrdersFromAllPagesAsync( string mainUpdatedOrdersEndpoint, Mark mark, CancellationToken token, int timeout )
		{
			var orders = new ShopifyOrders();
			var compositeUpdatedOrdersEndpoint = mainUpdatedOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit ) ) );

			do
			{
				var updatedOrdersWithinPage = await ActionPolicies.GetPolicyAsync( mark, this._shopName, token ).Get(
					() => this._throttlerAsync.ExecuteAsync(
						() => this._webRequestServices.GetResponsePageAsync< ShopifyOrders >( _shopifyCommandFactory.CreateGetOrdersCommand(), 
							compositeUpdatedOrdersEndpoint, token, mark, timeout ) ) );

				if( updatedOrdersWithinPage.Response.Orders.Count == 0 )
					break;

				foreach( var order in updatedOrdersWithinPage.Response.Orders )
					ProcessRefundOrderLineItems( order );

				orders.Orders.AddRange( updatedOrdersWithinPage.Response.Orders );

				compositeUpdatedOrdersEndpoint = updatedOrdersWithinPage.NextPageQueryStr;
			} while( compositeUpdatedOrdersEndpoint != string.Empty );

			return orders;
		}

		internal static void ProcessRefundOrderLineItems( ShopifyOrder order )
		{
			if ( order.Refunds == null || !order.Refunds.Any() )
				return;

			var actualOrderItems = new List< ShopifyOrderItem >();
			foreach( var orderItem in order.OrderItems )
			{
				var isCancelled = false;
				var cancelledQuantity = 0;

				foreach( var refund in order.Refunds )
				{
					var refundLineItem = refund.RefundLineItems.FirstOrDefault( rl => rl.LineItemId.ToString().Equals( orderItem.Id ) );
					if( refundLineItem == null )
						continue;

					// remove order item
					if ( orderItem.Quantity == refundLineItem.Quantity && refundLineItem.RestockType.Equals( "cancel" ) )
					{
						isCancelled = true;
						break;
					}
						
					// adjust quantity
					cancelledQuantity += refundLineItem.Quantity;
				}

				if( isCancelled )
					continue;
				
				orderItem.Quantity -= cancelledQuantity;
				actualOrderItems.Add( orderItem );
			}

			order.OrderItems = actualOrderItems;
		}

		#endregion

		#region Products
		public async Task< ShopifyProducts > GetProductsCreatedAfterAsync( DateTime productsStartUtc, CancellationToken token, Mark mark )
		{
			ShopifyLogger.LogOperationStart( this._shopName, mark, $"productsStartUtc: '{productsStartUtc}'" );

			try
			{
				var response = await this._graphQlPaginationService.GetAllPagesAsync< GetProductsData, Product >( 
					async (nextCursor) => await this._webRequestServices.PostDataAsync< GetProductsResponse >( this._shopifyCommandFactory.CreateGraphQlCommand(),
						QueryBuilder.GetProductsCreatedOnOrAfterRequest( productsStartUtc, nextCursor ),
						token, mark, this._timeouts[ ShopifyOperationEnum.GetProducts ] ),
					mark, token );

				return response?.ToShopifyProducts();
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}
		
		public async Task< ShopifyProducts > GetProductsCreatedBeforeButUpdatedAfterAsync( DateTime productsStartUtc, CancellationToken token, Mark mark )
		{
			ShopifyLogger.LogOperationStart( this._shopName, mark, $"productsStartUtc: '{productsStartUtc}'" );
			
			if( productsStartUtc == DateTime.MinValue )
			{
				return new ShopifyProducts();
			}

			try
			{
				var response = await this._graphQlPaginationService.GetAllPagesAsync< GetProductsData, Product >( 
					async (nextCursor) => await this._webRequestServices.PostDataAsync< GetProductsResponse >( this._shopifyCommandFactory.CreateGraphQlCommand(),
						QueryBuilder.GetProductsCreatedBeforeButUpdatedAfter( productsStartUtc, nextCursor ),
						token, mark, this._timeouts[ ShopifyOperationEnum.GetProducts ] ),
					mark, token );

				return response?.ToShopifyProducts();
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		public async Task< List< ShopifyProductVariant > > GetProductVariantsInventoryAsync( CancellationToken token, Mark mark )
		{
			var productVariants = await this.GetAllProductVariantsInventoryAsync( mark, token );
			var locations = await this.GetLocationsAsync( token, mark );
			var inventoryLevels = await this.CollectInventoryLevelsFromAllPagesAsync( mark, locations, token );

			foreach( var variant in productVariants )
			{
				var inventoryLevelsModelOfInventoryItemId = new List< ShopifyInventoryLevelModel >();
				if( !inventoryLevels.InventoryLevels.TryGetValue( variant.InventoryItemId, out inventoryLevelsModelOfInventoryItemId ) )
					continue;

				var inventoryLevelsOfInventoryItemId = inventoryLevelsModelOfInventoryItemId.Select( x => x.ToShopifyInventoryLevel( variant.InventoryItemId ) ).ToList();

				var inventoryLevelsForVariant = new ShopifyInventoryLevels { InventoryLevels = inventoryLevelsOfInventoryItemId };
				variant.InventoryLevels = inventoryLevelsForVariant;
			}

			return productVariants;
		}
		
		public async Task< List< ShopifyProductVariant > > GetProductVariantsInventoryBySkusAsync( IEnumerable< string > skus, CancellationToken token, Mark mark )
		{
			var productVariants = await this.GetAllProductVariantsInventoryAsync( mark, token );

			var variantIds = productVariants.Select( v => new { Sku = v.Sku.ToLowerInvariant(), v.InventoryItemId } );
			var inventoryItemIds = skus.Select( s => s.ToLowerInvariant() ).Distinct()
				.Join( variantIds, s => s, v => v.Sku, ( s, v ) => v.InventoryItemId )
				.ToArray();
			var inventoryLevels = await this.CollectInventoryLevelsFromAllPagesAsync( mark, inventoryItemIds, token, this._timeouts[ ShopifyOperationEnum.GetProductsInventory ] );
			productVariants = productVariants
				.Join( inventoryItemIds, v => v.InventoryItemId, iid => iid, ( v, iid ) => v )
				.ToList();

			foreach( var variant in productVariants )
			{
				List< ShopifyInventoryLevelModel > inventoryLevelsModelOfInventoryItemId;
				if( !inventoryLevels.InventoryLevels.TryGetValue( variant.InventoryItemId, out inventoryLevelsModelOfInventoryItemId ) )
					continue;

				var inventoryLevelsOfInventoryItemId = inventoryLevelsModelOfInventoryItemId.Select( x => x.ToShopifyInventoryLevel( variant.InventoryItemId ) ).ToList();

				var inventoryLevelsForVariant = new ShopifyInventoryLevels { InventoryLevels = inventoryLevelsOfInventoryItemId };
				variant.InventoryLevels = inventoryLevelsForVariant;
			}

			return productVariants;
		}

		private async Task< List< ProductVariant > > GetProductVariantsInventoryReportBySkuAsync( string sku, int locationsCount, Mark mark, CancellationToken token )
		{
			ShopifyLogger.LogOperationStart( this._shopName, mark, $"Sku: '{sku}'" );

			try
			{
				return await this._graphQlPaginationService.GetAllPagesAsync< GetProductVariantsInventoryData, ProductVariant >( 
					async (nextCursor) => await this._webRequestServices.PostDataAsync< GetProductVariantsInventoryResponse >( this._shopifyCommandFactory.CreateGraphQlCommand(),
						QueryBuilder.GetProductVariantInventoryBySkuRequest( sku, nextCursor, locationsCount ),
						token, mark, this._timeouts[ ShopifyOperationEnum.GetProductsInventory ] ),
					mark, token );
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		public async Task< List< ShopifyProductVariant > > GetProductVariantsInventoryReportBySkusAsync( IEnumerable< string > skus, CancellationToken token, Mark mark )
		{
			ShopifyLogger.LogOperationStart( this._shopName, mark );

			try
			{
				var locations = await this.GetLocationsAsync( token, mark ).ConfigureAwait( false );
				var locationsCount = locations.Locations.Count;
				var correctedSkus = skus.Select( s => s.ToLowerInvariant() ).Distinct();

				var result = new List< ProductVariant >();
				foreach( var sku in correctedSkus )
				{
					var productVariantsBySku = await this.GetProductVariantsInventoryReportBySkuAsync( sku, locationsCount, mark, token );
					result.AddRange( productVariantsBySku );
				}

				return new List< ShopifyProductVariant >( result.Where( FilterProductVariants ).Select( variant => variant.ToShopifyProductVariantForInventory() ) );
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		public async Task< List< ShopifyProductVariant > > GetProductVariantsInventoryReportAsync( CancellationToken token, Mark mark )
		{
			ShopifyLogger.LogOperationStart( this._shopName, mark );

			try
			{
				var data = await this._reportGenerator.GetReportAsync(
					ReportType.ProductVariantsInventory,
					ProductVariantsInventoryReportParser.Parse,
					this._timeouts[ ShopifyOperationEnum.GetProductsInventory ],
					mark, token ).ConfigureAwait( false );
				return new List< ShopifyProductVariant >( data.Where( FilterProductVariants ).Select( variant => variant.ToShopifyProductVariantForInventory() ) );
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		/// <summary>
		/// Filter out variants that have inventory untracked or empty SKU
		/// </summary>
		/// <param name="variant"></param>
		/// <returns></returns>
		private static bool FilterProductVariants( ProductVariant variant )
		{
			return ( variant?.InventoryItem?.Tracked ?? false ) && !string.IsNullOrEmpty( variant.Sku );
		}

		private async Task< List< ShopifyProductVariant > > GetAllProductVariantsInventoryAsync( Mark mark, CancellationToken token )
		{
			ShopifyLogger.LogOperationStart( this._shopName, mark );
			
			try
			{
				var response = await this._graphQlPaginationService.GetAllPagesAsync< GetProductVariantsInventoryData, ProductVariant >( 
					async (nextCursor) => await this._webRequestServices.PostDataAsync< GetProductVariantsInventoryResponse >( this._shopifyCommandFactory.CreateGraphQlCommand(),
						QueryBuilder.GetAllProductVariants( nextCursor ),
						token, mark, this._timeouts[ ShopifyOperationEnum.GetProductsInventory ] ),
					mark, token );
				
				return response?
					.Where( FilterProductVariants )
					.Select( y => y.ToShopifyProductVariantForInventory() ).ToList() ?? new List< ShopifyProductVariant >();
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		private void ConvertToShopifyInventoryLevelsModel( ShopifyInventoryLevelsModel inventoryLevels, ShopifyInventoryLevels productsWithinPage )
		{
			var productsWithinPageGroupByInventoryItemId = productsWithinPage.InventoryLevels
				.GroupBy( x => x.InventoryItemId )
				.Select( x => new
				{
					InventoryItemId = x.Key,
					Data = x.Select( t => new ShopifyInventoryLevelModel( t ) ).ToList()
				} )
				.ToList();

			foreach( var item in productsWithinPageGroupByInventoryItemId )
				if( !inventoryLevels.InventoryLevels.ContainsKey( item.InventoryItemId ) )
					inventoryLevels.InventoryLevels.Add( item.InventoryItemId, item.Data );
				else
					inventoryLevels.InventoryLevels[ item.InventoryItemId ].AddRange( item.Data );
		}

		private async Task< ShopifyInventoryLevelsModel > CollectInventoryLevelsFromAllPagesAsync( Mark mark, long[] productIds, CancellationToken token, int timeout )
		{
			var inventoryLevels = new ShopifyInventoryLevelsModel();
			var partsOfProductIds = productIds.Slice( RequestInventoryLevelsMaxLimit );

			foreach( var ids in partsOfProductIds )
			{
				var endpoint = EndpointsBuilder.CreateInventoryLevelsIdsEndpoint( ids, RequestMaxLimit );

				do
				{
					var productsWithinPage = await ActionPolicies.GetPolicyAsync( mark, this._shopName, token ).Get(
						() => this._throttlerAsync.ExecuteAsync(
							() => this._webRequestServices.GetResponsePageAsync< ShopifyInventoryLevels >( _shopifyCommandFactory.CreateGetInventoryLevelsCommand(), endpoint, token, mark, timeout ) ) );
					
					this.ConvertToShopifyInventoryLevelsModel( inventoryLevels, productsWithinPage.Response );

					if( productsWithinPage.Response.InventoryLevels.Count < RequestMaxLimit )
						break;

					endpoint = productsWithinPage.NextPageQueryStr;
				} while( endpoint != string.Empty );
			}

			return inventoryLevels;
		}

		private async Task< ShopifyInventoryLevelsModel > CollectInventoryLevelsFromAllPagesAsync( Mark mark, ShopifyLocations shopifyLocations, CancellationToken token )
		{
			var inventoryLevels = new ShopifyInventoryLevelsModel();
			var partsOfLocationIds = shopifyLocations.Locations.Select( x => x.Id ).Slice( RequestInventoryLevelsMaxLimit );

			foreach( var ids in partsOfLocationIds )
			{
				var endpoint = EndpointsBuilder.CreateInventoryLevelsIdsEndpoint( ids, RequestMaxLimit );

				do
				{
					var productsWithinPage = await ActionPolicies.GetPolicyAsync( mark, this._shopName, token ).Get(
						() => this._throttlerAsync.ExecuteAsync(
							() => this._webRequestServices.GetResponsePageAsync< ShopifyInventoryLevels >( _shopifyCommandFactory.CreateGetInventoryLevelsCommand(), endpoint, token, mark, this._timeouts[ ShopifyOperationEnum.GetProductsInventory ] ) ) );

					this.ConvertToShopifyInventoryLevelsModel( inventoryLevels, productsWithinPage.Response );

					if( productsWithinPage.Response.InventoryLevels.Count < RequestMaxLimit )
						break;

					endpoint = productsWithinPage.NextPageQueryStr;
				} while( endpoint != String.Empty );
			}

			return inventoryLevels;
		}
		#endregion

		#region Update variants
		public async Task UpdateInventoryLevelsAsync( IEnumerable< ShopifyInventoryLevelForUpdate > inventoryLevels, CancellationToken token, Mark mark )
		{
			foreach( var inventoryLevel in inventoryLevels )
				await this.UpdateInventoryLevelQuantityAsync( inventoryLevel, token, mark );
		}

		private async Task UpdateInventoryLevelQuantityAsync( ShopifyInventoryLevelForUpdate variant, CancellationToken token, Mark mark )
		{
			//just simpliest way to serialize with the root name.
			var jsonContent = variant.ToJson();

			await ActionPolicies.SubmitPolicyAsync( mark, this._shopName, token ).Do( async () =>
				await this._productUpdateThrottlerAsync.ExecuteAsync( async () =>
				{
					await this._webRequestServices.PostDataAsync< ShopifyInventoryLevelForUpdateResponse >( _shopifyCommandFactory.CreateUpdateInventoryLevelsCommand(), jsonContent, token, mark, this._timeouts[ ShopifyOperationEnum.UpdateInventory ] );
					return true;
				} ) );
		}
		#endregion

		#region Users
		public ShopifyUsers GetUsers( CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var users = ActionPolicies.GetPolicy( mark, this._shopName, token ).Get(
				() => this._throttler.Execute(
					() => this._webRequestServices.GetResponse< ShopifyUsers >( _shopifyCommandFactory.CreateGetUsersCommand(), "", token, mark, this._timeouts[ ShopifyOperationEnum.GetUsers ] ) ) );
			return users;
		}

		public Task< ShopifyUsers > GetUsersAsync( CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var users = ActionPolicies.GetPolicyAsync( mark, this._shopName, token ).Get(
				() => this._throttlerAsync.ExecuteAsync(
					() => this._webRequestServices.GetResponseAsync< ShopifyUsers >( _shopifyCommandFactory.CreateGetUsersCommand(), "", token, mark, this._timeouts[ ShopifyOperationEnum.GetUsers ] ) ) );
			return users;
		}

		public bool IsShopifyPlusAccount(  CancellationToken token, Mark mark = null )
		{
			try
			{
				mark = mark.CreateNewIfBlank();
				var users = this._throttler.Execute(
					() => this._webRequestServices.GetResponse< ShopifyUsers >( _shopifyCommandFactory.CreateGetUsersCommand(), "", token, mark, this._timeouts[ ShopifyOperationEnum.GetUsers ] ) );
				return users?.Users?.FirstOrDefault() != null;
			}
			catch( Exception )
			{
				return false;
			}
		}

		public async Task< bool > IsShopifyPlusAccountAsync( CancellationToken token, Mark mark = null )
		{
			try
			{
				mark = mark.CreateNewIfBlank();
				var users = await this._throttlerAsync.ExecuteAsync(
					() => this._webRequestServices.GetResponseAsync< ShopifyUsers >( _shopifyCommandFactory.CreateGetUsersCommand(), string.Empty, token, mark, this._timeouts[ ShopifyOperationEnum.GetUsers ] ) );
				return users?.Users?.FirstOrDefault() != null;
			}
			catch( Exception )
			{
				return false;
			}
		}
		#endregion
	}
}