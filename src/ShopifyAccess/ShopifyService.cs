using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Netco.Extensions;
using ServiceStack;
using ShopifyAccess.GraphQl;
using ShopifyAccess.GraphQl.Models.ProductVariantsInventoryReport;
using ShopifyAccess.GraphQl.Models.ProductVariantsInventoryReport.Extensions;
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

		// Separate throttler for updating to save limit for other syncs
		private readonly ShopifyThrottler _productUpdateThrottler = new ShopifyThrottler( 30 );
		private readonly ShopifyThrottlerAsync _productUpdateThrottlerAsync = new ShopifyThrottlerAsync( 30 );
		private readonly ShopifyTimeouts _timeouts;

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

		public ShopifyService( ShopifyCommandConfig config, ShopifyTimeouts operationsTimeouts )
		{
			Condition.Requires( config, "config" ).IsNotNull();
			Condition.Requires( operationsTimeouts, "operationsTimeouts" ).IsNotNull();

			this._webRequestServices = new WebRequestServices( config );
			this._reportGenerator = new ReportGenerator( config.ShopName, this._webRequestServices);
			this._shopName = config.ShopName;
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
			var locations = ActionPolicies.GetPolicy( mark, this._shopName ).Get(
				() => this._throttler.Execute(
					() => this._webRequestServices.GetResponse< ShopifyLocations >( ShopifyCommand.GetLocations, "", token, mark, this._timeouts[ ShopifyOperationEnum.GetLocations ] ) ) );
			return locations;
		}

		public Task< ShopifyLocations > GetLocationsAsync( CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var locations = ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
				() => this._throttlerAsync.ExecuteAsync(
					() => this._webRequestServices.GetResponseAsync< ShopifyLocations >( ShopifyCommand.GetLocations, "", token, mark, this._timeouts[ ShopifyOperationEnum.GetLocations ] ) ) );
			return locations;
		}

		private ShopifyOrders CollectOrdersFromAllPages( string mainUpdatedOrdersEndpoint, Mark mark, CancellationToken token, int timeout )
		{
			var orders = new ShopifyOrders();
			var compositeUpdatedOrdersEndpoint = mainUpdatedOrdersEndpoint.ConcatEndpoints( EndpointsBuilder.CreateGetEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit ) ) );

			do
			{
				var updatedOrdersWithinPage = ActionPolicies.GetPolicy( mark, this._shopName ).Get(
					() => this._throttler.Execute(
						() => this._webRequestServices.GetResponsePage< ShopifyOrders >( ShopifyCommand.GetOrders, compositeUpdatedOrdersEndpoint, token, mark, timeout ) ) );

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
				var updatedOrdersWithinPage = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
					() => this._throttlerAsync.ExecuteAsync(
						() => this._webRequestServices.GetResponsePageAsync< ShopifyOrders >( ShopifyCommand.GetOrders, compositeUpdatedOrdersEndpoint, token, mark, timeout ) ) );

				if( updatedOrdersWithinPage.Response.Orders.Count == 0 )
					break;

				foreach( var order in updatedOrdersWithinPage.Response.Orders )
					this.RemoveCancelledOrderItems( order );

				orders.Orders.AddRange( updatedOrdersWithinPage.Response.Orders );

				compositeUpdatedOrdersEndpoint = updatedOrdersWithinPage.NextPageQueryStr;
			} while( compositeUpdatedOrdersEndpoint != string.Empty );

			return orders;
		}

		private void RemoveCancelledOrderItems( ShopifyOrder order )
		{
			if ( order.Refunds == null || !order.Refunds.Any() )
				return;

			var actualOrderItems = new List< ShopifyOrderItem >();
			foreach( var orderItem in order.OrderItems )
			{
				bool isCancelled = false;
				int cancelledQuantity = 0;

				foreach( var refund in order.Refunds )
				{
					var refundLineItem = refund.RefundLineItems.FirstOrDefault( rl => rl.LineItemId.ToString().Equals( orderItem.Id ) );

					if ( refundLineItem != null && refundLineItem.RestockType.Equals( "cancel" ) )
					{
						// remove order item
						if ( orderItem.Quantity == refundLineItem.Quantity )
						{
							isCancelled = true;
							break;
						}
						
						// adjust quantity
						cancelledQuantity += refundLineItem.Quantity;
					}
				}

				if ( !isCancelled )
				{
					orderItem.Quantity -= cancelledQuantity;
					actualOrderItems.Add( orderItem );
				}
			}

			order.OrderItems = actualOrderItems;
		}

		private int GetOrdersCount( string updatedOrdersEndpoint, Mark mark, CancellationToken token )
		{
			var count = ActionPolicies.GetPolicy( mark, this._shopName ).Get(
				() => this._throttler.Execute(
					() => this._webRequestServices.GetResponse< OrdersCount >( ShopifyCommand.GetOrdersCount, updatedOrdersEndpoint, token, mark, this._timeouts[ ShopifyOperationEnum.GetOrdersCount ] ).Count ) );
			return count;
		}

		private Task< int > GetOrdersCountAsync( string updatedOrdersEndpoint, Mark mark, CancellationToken token )
		{
			var count = ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
				() => this._throttlerAsync.ExecuteAsync(
					async () => ( await this._webRequestServices.GetResponseAsync< OrdersCount >( ShopifyCommand.GetOrdersCount, updatedOrdersEndpoint, token, mark, this._timeouts[ ShopifyOperationEnum.GetOrdersCount ] ) ).Count ) );
			return count;
		}
		#endregion

		#region Products
		public ShopifyProducts GetProducts( CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();

			var timeout = this._timeouts[ ShopifyOperationEnum.GetProducts ];
			var products = this.CollectProductsFromAllPages( token, mark, timeout );
			this.RemoveUntrackedProductVariants( products );
			var inventoryLevels = this.CollectInventoryLevelsFromAllPages( token, mark, products.Products.SelectMany( x => x.Variants.Select( t => t.InventoryItemId ) ).ToArray(), timeout );

			foreach( var product in products.Products )
			foreach( var variant in product.Variants )
			{
				var inventoryLevelsModelOfInventoryItemId = new List< ShopifyInventoryLevelModel >();
				if( !inventoryLevels.InventoryLevels.TryGetValue( variant.InventoryItemId, out inventoryLevelsModelOfInventoryItemId ) )
					continue;

				var inventoryLevelsOfInventoryItemId = inventoryLevelsModelOfInventoryItemId.Select( x => x.ToShopifyInventoryLevel( variant.InventoryItemId ) ).ToList();

				var inventoryLevelsForVariant = new ShopifyInventoryLevels { InventoryLevels = inventoryLevelsOfInventoryItemId };
				variant.InventoryLevels = inventoryLevelsForVariant;
			}

			RemoveQueryPartFromProductsImagesUrl( products );

			return products;
		}

		public async Task< ShopifyProducts > GetProductsAsync( CancellationToken token, Mark mark = null )
		{
			return await this.GetProductsCreatedAfterAsync( DateTime.MinValue, token, mark );
		}
		
		public async Task< ShopifyProducts > GetProductsCreatedAfterAsync( DateTime productsStartUtc, CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();

			var productsDateFilter = new ProductsDateFilter
			{
				FilterType = productsStartUtc != DateTime.MinValue ? FilterType.CreatedAfter : FilterType.None,
				ProductsStartUtc = productsStartUtc
			};
			var products = await this.CollectProductsFromAllPagesAsync( productsDateFilter, mark, token );
			
			RemoveQueryPartFromProductsImagesUrl( products );

			return products;
		}
		
		public async Task< ShopifyProducts > GetProductsCreatedBeforeButUpdatedAfterAsync( DateTime productsStartUtc, CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();

			if( productsStartUtc == DateTime.MinValue )
			{
				return new ShopifyProducts();
			}

			var productsDateFilter = new ProductsDateFilter
			{
				FilterType = FilterType.CreatedBeforeUpdatedAfter,
				ProductsStartUtc = productsStartUtc
			};
			var products = await this.CollectProductsFromAllPagesAsync( productsDateFilter, mark, token );

			RemoveQueryPartFromProductsImagesUrl( products );

			return products;
		}

		public ShopifyProducts GetProductsInventory( CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();

			var products = this.CollectProductsFromAllPages( token, mark, this._timeouts[ ShopifyOperationEnum.GetProducts ] );
			var locations = this.GetLocations( token, mark );
			this.RemoveUntrackedProductVariants( products );
			var inventoryLevels = this.CollectInventoryLevelsFromAllPages( token, mark, locations, this._timeouts[ ShopifyOperationEnum.GetProductsInventory ] );

			foreach( var product in products.Products )
			foreach( var variant in product.Variants )
			{
				var inventoryLevelsModelOfInventoryItemId = new List< ShopifyInventoryLevelModel >();
				if( !inventoryLevels.InventoryLevels.TryGetValue( variant.InventoryItemId, out inventoryLevelsModelOfInventoryItemId ) )
					continue;

				var inventoryLevelsOfInventoryItemId = inventoryLevelsModelOfInventoryItemId.Select( x => x.ToShopifyInventoryLevel( variant.InventoryItemId ) ).ToList();

				var inventoryLevelsForVariant = new ShopifyInventoryLevels { InventoryLevels = inventoryLevelsOfInventoryItemId };
				variant.InventoryLevels = inventoryLevelsForVariant;
			}

			return products;
		}

		public async Task< ShopifyProducts > GetProductsInventoryAsync( CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();

			var products = await this.CollectProductsFromAllPagesAsync( mark, token );
			var locations = await this.GetLocationsAsync( token, mark );
			this.RemoveUntrackedProductVariants( products );
			var inventoryLevels = await this.CollectInventoryLevelsFromAllPagesAsync( mark, locations, token );

			foreach( var product in products.Products )
			foreach( var variant in product.Variants )
			{
				var inventoryLevelsModelOfInventoryItemId = new List< ShopifyInventoryLevelModel >();
				if( !inventoryLevels.InventoryLevels.TryGetValue( variant.InventoryItemId, out inventoryLevelsModelOfInventoryItemId ) )
					continue;

				var inventoryLevelsOfInventoryItemId = inventoryLevelsModelOfInventoryItemId.Select( x => x.ToShopifyInventoryLevel( variant.InventoryItemId ) ).ToList();

				var inventoryLevelsForVariant = new ShopifyInventoryLevels { InventoryLevels = inventoryLevelsOfInventoryItemId };
				variant.InventoryLevels = inventoryLevelsForVariant;
			}

			return products;
		}

		public async Task< List< ShopifyProductVariant > > GetProductVariantsInventoryBySkusAsync( IEnumerable< string > skus, CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();

			var products = await this.CollectProductsFromAllPagesAsync( mark, token );
			this.RemoveUntrackedProductVariants( products );

			var productVariants = products.ToListVariants();
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

		public async Task< List< ShopifyProductVariant > > GetProductVariantsInventoryReportAsync( CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();

			ShopifyLogger.LogOperationStart( this._shopName, mark );

			try
			{
				var data = await this._reportGenerator.GetReportAsync(
					ReportType.ProductVariantsInventory,
					ProductVariantsInventoryReportParser.Parse,
					this._timeouts[ ShopifyOperationEnum.GetProductsInventory ],
					mark, token ).ConfigureAwait( false );
				return new List< ShopifyProductVariant >( data.Where( FilterProductVariants ).Select( variant => variant.ToShopifyProductVariant() ) );
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		private static bool FilterProductVariants( ProductVariant variant )
		{
			return variant.InventoryItem.Tracked && !string.IsNullOrEmpty( variant.Sku );
		}

		private int GetProductsCount( Mark mark, CancellationToken token )
		{
			var count = ActionPolicies.GetPolicy( mark, this._shopName ).Get(
				() => this._throttler.Execute(
					() => this._webRequestServices.GetResponse< ProductsCount >( ShopifyCommand.GetProductsCount, EndpointsBuilder.EmptyEndpoint, token, mark, this._timeouts[ ShopifyOperationEnum.GetProductsCount ] ).Count ) );
			return count;
		}

		private Task< int > GetProductsCountAsync( Mark mark, CancellationToken token )
		{
			var count = ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
				() => this._throttlerAsync.ExecuteAsync(
					async () => ( await this._webRequestServices.GetResponseAsync< ProductsCount >( ShopifyCommand.GetProductsCount, EndpointsBuilder.EmptyEndpoint, token, mark, this._timeouts[ ShopifyOperationEnum.GetProductsCount ] ) ).Count ) );
			return count;
		}

		private ShopifyProducts CollectProductsFromAllPages( CancellationToken token, Mark mark, int timeout )
		{
			var products = new ShopifyProducts();
			var endpoint = EndpointsBuilder.CreateGetEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit ) );

			do
			{
				var productsWithinPage = ActionPolicies.GetPolicy( mark, this._shopName ).Get(
					() => this._throttler.Execute(
						() => this._webRequestServices.GetResponsePage< ShopifyProducts >( ShopifyCommand.GetProducts, endpoint, token, mark, timeout ) ) );

				if( productsWithinPage.Response.Products.Count == 0 )
					break;

				products.Products.AddRange( productsWithinPage.Response.Products );

				endpoint = productsWithinPage.NextPageQueryStr;
			} while( endpoint != string.Empty );

			return products;
		}

		private async Task< ShopifyProducts > CollectProductsFromAllPagesAsync( Mark mark, CancellationToken token )
		{
			var noFilter = new ProductsDateFilter { FilterType = FilterType.None };

			return await this.CollectProductsFromAllPagesAsync( noFilter, mark, token );
		}

		private async Task< ShopifyProducts > CollectProductsFromAllPagesAsync( ProductsDateFilter productsDateFilter, Mark mark, CancellationToken token )
		{
			var products = new ShopifyProducts();
			var endpoint = EndpointsBuilder.CreateGetEndpoint( new ShopifyCommandEndpointConfig( RequestMaxLimit ) );

			if( productsDateFilter.FilterType != FilterType.None )
			{
				endpoint += EndpointsBuilder.AppendGetProductsFilteredByDateEndpoint( productsDateFilter, endpoint );
			}

			do
			{
				var productsWithinPage = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
					() => this._throttlerAsync.ExecuteAsync(
						() => this._webRequestServices.GetResponsePageAsync< ShopifyProducts >( ShopifyCommand.GetProducts, endpoint, token, mark, this._timeouts[ ShopifyOperationEnum.GetProducts ] ) ) );
				if( productsWithinPage.Response.Products.Count == 0 )
					break;

				products.Products.AddRange( productsWithinPage.Response.Products );

				endpoint = productsWithinPage.NextPageQueryStr;
			} while( endpoint != string.Empty );

			return products;
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

		private ShopifyInventoryLevelsModel CollectInventoryLevelsFromAllPages( CancellationToken token, Mark mark, long[] productIds, int timeout )
		{
			var inventoryLevels = new ShopifyInventoryLevelsModel();
			var partsOfProductIds = productIds.Slice( RequestInventoryLevelsMaxLimit );

			foreach( var ids in partsOfProductIds )
			{
				var endpoint = EndpointsBuilder.CreateInventoryLevelsIdsEndpoint( ids, RequestMaxLimit );

				do
				{
					var productsWithinPage = ActionPolicies.GetPolicy( mark, this._shopName ).Get(
						() => this._throttler.Execute(
							() => this._webRequestServices.GetResponsePage< ShopifyInventoryLevels >( ShopifyCommand.GetInventoryLevels, endpoint, token, mark, timeout ) ) );

					this.ConvertToShopifyInventoryLevelsModel( inventoryLevels, productsWithinPage.Response );

					if( productsWithinPage.Response.InventoryLevels.Count < RequestMaxLimit )
						break;

					endpoint = productsWithinPage.NextPageQueryStr;
				} while( endpoint != string.Empty );
			}

			return inventoryLevels;
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
					var productsWithinPage = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
						() => this._throttlerAsync.ExecuteAsync(
							() => this._webRequestServices.GetResponsePageAsync< ShopifyInventoryLevels >( ShopifyCommand.GetInventoryLevels, endpoint, token, mark, timeout ) ) );
					
					this.ConvertToShopifyInventoryLevelsModel( inventoryLevels, productsWithinPage.Response );

					if( productsWithinPage.Response.InventoryLevels.Count < RequestMaxLimit )
						break;

					endpoint = productsWithinPage.NextPageQueryStr;
				} while( endpoint != string.Empty );
			}

			return inventoryLevels;
		}

		private ShopifyInventoryLevelsModel CollectInventoryLevelsFromAllPages( CancellationToken token, Mark mark, ShopifyLocations shopifyLocations, int timeout )
		{
			var inventoryLevels = new ShopifyInventoryLevelsModel();
			var partsOfLocationIds = shopifyLocations.Locations.Select( x => x.Id ).Slice( RequestInventoryLevelsMaxLimit );

			foreach( var ids in partsOfLocationIds )
			{
				var endpoint = EndpointsBuilder.CreateInventoryLevelsIdsEndpoint( ids, RequestMaxLimit );

				do
				{
					var productsWithinPage = ActionPolicies.GetPolicy( mark, this._shopName ).Get(
						() => this._throttler.Execute(
							() => this._webRequestServices.GetResponsePage< ShopifyInventoryLevels >( ShopifyCommand.GetInventoryLevels, endpoint, token, mark, timeout ) ) );

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

					var productsWithinPage = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
						() => this._throttlerAsync.ExecuteAsync(
							() => this._webRequestServices.GetResponsePageAsync< ShopifyInventoryLevels >( ShopifyCommand.GetInventoryLevels, endpoint, token, mark, this._timeouts[ ShopifyOperationEnum.GetProductsInventory ] ) ) );

					this.ConvertToShopifyInventoryLevelsModel( inventoryLevels, productsWithinPage.Response );

					if( productsWithinPage.Response.InventoryLevels.Count < RequestMaxLimit )
						break;

					endpoint = productsWithinPage.NextPageQueryStr;
				} while( endpoint != String.Empty );
			}

			return inventoryLevels;
		}

		private void RemoveUntrackedProductVariants( ShopifyProducts products )
		{
			foreach( var product in products.Products )
			{
				product.Variants.RemoveAll( v => v.InventoryManagement == InventoryManagement.Blank );
			}
		}

		private void RemoveQueryPartFromProductsImagesUrl( ShopifyProducts products )
		{
			foreach( var product in products.Products )
			{
				if ( product.Images == null )
					return;
			
				foreach( var productImage in product.Images )
				{
					productImage.Src = productImage.Src.GetUrlWithoutQueryPart();
				}
			}
		}
		#endregion

		#region Update variants
		public void UpdateProductVariants( IEnumerable< ShopifyProductVariantForUpdate > variants, CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			foreach( var variant in variants )
			{
				this.UpdateProductVariantQuantity( variant, token, mark );
			}
		}
		
		public async Task UpdateProductVariantsAsync( IEnumerable< ShopifyProductVariantForUpdate > variants, CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			foreach( var variant in variants )
			{
				await this.UpdateProductVariantQuantityAsync( variant, token, mark );
			}
		}

		private void UpdateProductVariantQuantity( ShopifyProductVariantForUpdate variant, CancellationToken token, Mark mark )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			//just simpliest way to serialize with the root name.
			var jsonContent = new { variant }.ToJson();

			ActionPolicies.SubmitPolicy( mark, this._shopName ).Do( () =>
				this._productUpdateThrottler.Execute( () =>
				{
					this._webRequestServices.PutData( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent, token, mark, this._timeouts[ ShopifyOperationEnum.UpdateProductVariantQuantity ] );
					return true;
				} ) );
		}

		private async Task UpdateProductVariantQuantityAsync( ShopifyProductVariantForUpdate variant, CancellationToken token, Mark mark )
		{
			var endpoint = EndpointsBuilder.CreateProductVariantUpdateEndpoint( variant.Id );
			var jsonContent = new { variant }.ToJson();

			await ActionPolicies.SubmitPolicyAsync( mark, this._shopName ).Do( async () =>
				await this._productUpdateThrottlerAsync.ExecuteAsync( async () =>
				{
					await this._webRequestServices.PutDataAsync( ShopifyCommand.UpdateProductVariant, endpoint, jsonContent, token, mark, this._timeouts[ ShopifyOperationEnum.UpdateProductVariantQuantity ] );
					return true;
				} ) );
		}
		
		public void UpdateInventoryLevels( IEnumerable< ShopifyInventoryLevelForUpdate > inventoryLevels, CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			foreach( var inventoryLevel in inventoryLevels )
				this.UpdateInventoryLevelQuantity( inventoryLevel, token, mark );
		}
		
		public async Task UpdateInventoryLevelsAsync( IEnumerable< ShopifyInventoryLevelForUpdate > inventoryLevels, CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			foreach( var inventoryLevel in inventoryLevels )
				await this.UpdateInventoryLevelQuantityAsync( inventoryLevel, token, mark );
		}

		private void UpdateInventoryLevelQuantity( ShopifyInventoryLevelForUpdate variant, CancellationToken token, Mark mark )
		{
			//just simpliest way to serialize with the root name.
			var jsonContent = variant.ToJson();

			ActionPolicies.SubmitPolicy( mark, this._shopName ).Do( () =>
				this._productUpdateThrottler.Execute( () =>
				{
					this._webRequestServices.PostData< ShopifyInventoryLevelForUpdateResponse >( ShopifyCommand.UpdateInventoryLevels, jsonContent, token, mark, this._timeouts[ ShopifyOperationEnum.UpdateInventory ] );
					return true;
				} ) );
		}

		private async Task UpdateInventoryLevelQuantityAsync( ShopifyInventoryLevelForUpdate variant, CancellationToken token, Mark mark )
		{
			//just simpliest way to serialize with the root name.
			var jsonContent = variant.ToJson();

			await ActionPolicies.SubmitPolicyAsync( mark, this._shopName ).Do( async () =>
				await this._productUpdateThrottlerAsync.ExecuteAsync( async () =>
				{
					await this._webRequestServices.PostDataAsync< ShopifyInventoryLevelForUpdateResponse >( ShopifyCommand.UpdateInventoryLevels, jsonContent, token, mark, this._timeouts[ ShopifyOperationEnum.UpdateInventory ] );
					return true;
				} ) );
		}
		#endregion

		#region Users
		public ShopifyUsers GetUsers( CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var users = ActionPolicies.GetPolicy( mark, this._shopName ).Get(
				() => this._throttler.Execute(
					() => this._webRequestServices.GetResponse< ShopifyUsers >( ShopifyCommand.GetUsers, "", token, mark, this._timeouts[ ShopifyOperationEnum.GetUsers ] ) ) );
			return users;
		}

		public Task< ShopifyUsers > GetUsersAsync( CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var users = ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
				() => this._throttlerAsync.ExecuteAsync(
					() => this._webRequestServices.GetResponseAsync< ShopifyUsers >( ShopifyCommand.GetUsers, "", token, mark, this._timeouts[ ShopifyOperationEnum.GetUsers ] ) ) );
			return users;
		}

		public ShopifyUser GetUser( long id, CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var user = ActionPolicies.GetPolicy( mark, this._shopName ).Get(
				() => this._throttler.Execute(
					() => this._webRequestServices.GetResponse< ShopifyUserWrapper >( ShopifyCommand.GetUser, EndpointsBuilder.CreateGetUserEndpoint( id ), token, mark, this._timeouts[ ShopifyOperationEnum.GetUser ] ) ) );
			return user.User;
		}

		public async Task< ShopifyUser > GetUserAsync( long id, CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			var user = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
				() => this._throttlerAsync.ExecuteAsync(
					() => this._webRequestServices.GetResponseAsync< ShopifyUserWrapper >( ShopifyCommand.GetUser, EndpointsBuilder.CreateGetUserEndpoint( id ), token, mark, this._timeouts[ ShopifyOperationEnum.GetUser ] ) ) );
			return user.User;
		}

		public bool IsShopifyPlusAccount(  CancellationToken token, Mark mark = null )
		{
			try
			{
				mark = mark.CreateNewIfBlank();
				var users = this._throttler.Execute(
					() => this._webRequestServices.GetResponse< ShopifyUsers >( ShopifyCommand.GetUsers, "", token, mark, this._timeouts[ ShopifyOperationEnum.GetUsers ] ) );
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
					() => this._webRequestServices.GetResponseAsync< ShopifyUsers >( ShopifyCommand.GetUsers, string.Empty, token, mark, this._timeouts[ ShopifyOperationEnum.GetUsers ] ) );
				return users?.Users?.FirstOrDefault() != null;
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
		#endregion
	}
}