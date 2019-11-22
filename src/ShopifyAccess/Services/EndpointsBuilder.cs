using System;
using System.Linq;
using System.Text;
using ServiceStack;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.Product;

namespace ShopifyAccess.Services
{
	public static class EndpointsBuilder
	{
		public static readonly string EmptyEndpoint = string.Empty;

		public static string CreateNewOrdersEndpoint( ShopifyOrderStatus status, DateTime startDate, DateTime endDate )
		{
			var endpoint = string.Format( "?{0}={1}&{2}={3}&{4}={5}",
				ShopifyOrderCommandEndpointName.OrderStatus.Name, status,
				ShopifyOrderCommandEndpointName.OrdersDateFrom.Name, DateTime.SpecifyKind( startDate, DateTimeKind.Utc ).ToString( "o" ),
				ShopifyOrderCommandEndpointName.OrdersDateTo.Name, DateTime.SpecifyKind( endDate, DateTimeKind.Utc ).ToString( "o" )
				);
			return endpoint;
		}

		public static string CreateUpdatedOrdersEndpoint( ShopifyOrderStatus status, DateTime startDate, DateTime endDate )
		{
			var endpoint = string.Format( "?{0}={1}&{2}={3}&{4}={5}",
				ShopifyOrderCommandEndpointName.OrderStatus.Name, status,
				ShopifyOrderCommandEndpointName.OrdersDateUpdatedFrom.Name, DateTime.SpecifyKind( startDate, DateTimeKind.Utc ).ToString( "o" ),
				ShopifyOrderCommandEndpointName.OrdersDateUpdatedTo.Name, DateTime.SpecifyKind( endDate, DateTimeKind.Utc ).ToString( "o" ));
			return endpoint;
		}

		public static string AppendGetProductsFilteredByDateEndpoint( ProductsDateFilter productsDateFilter, string initialEndpoint )
		{
			if( productsDateFilter.FilterType == FilterType.None )
				return string.Empty;

			var endpoint = !initialEndpoint.IsNullOrEmpty() ? "&" : "";

			switch( productsDateFilter.FilterType )
			{
				case FilterType.CreatedAfter:
					endpoint += string.Format( "{0}={1}",
						ShopifyProductCommandEndpointName.ProductDateCreatedAfter.Name, DateTime.SpecifyKind( productsDateFilter.ProductsStartUtc, DateTimeKind.Utc ).ToString( "o" ));
					break;
				case FilterType.CreatedBeforeUpdatedAfter:
					endpoint += string.Format( "{0}={1}&{2}={3}",
						ShopifyProductCommandEndpointName.ProductDateCreatedBefore.Name, DateTime.SpecifyKind( productsDateFilter.ProductsStartUtc, DateTimeKind.Utc ).ToString( "o" ),
						ShopifyProductCommandEndpointName.ProductDateUpdatedAfter.Name, DateTime.SpecifyKind( productsDateFilter.ProductsStartUtc, DateTimeKind.Utc ).ToString( "o" ));
					break;
			}

			return endpoint;
		}

		public static string CreateProductVariantUpdateEndpoint( long variantId )
		{
			var endpoint = string.Format( "{0}.json", variantId );
			return endpoint;
		}

		public static string CreateGetSinglePageEndpoint( ShopifyCommandEndpointConfig config )
		{
			var endpoint = string.Format( "?{0}={1}",
				ShopifyCommandEndpointName.Limit.Name, config.Limit
				);
			return endpoint;
		}

		public static string CreateGetNextPageSinceIdEndpoint( ShopifyCommandEndpointConfig config )
		{
			var endpoint = string.Format( "?{0}={1}&{2}={3}",
				ShopifyCommandEndpointName.Limit.Name, config.Limit,
				ShopifyCommandEndpointName.SinceId.Name, config.SinceId
			);
			return endpoint;
		}

		//TODO GUARD-220 Convert to cursors. "page=" in the Url is getting depricated
		public static string CreateInventoryLevelsIdsEndpoint( long[] ids, int page, int limit )
		{
			// INFO : limit should be 250. Page start from 1. Max of inventory_item_ids is 50.
			var endpoint = string.Format( "?inventory_item_ids={0}&page={1}&limit={2}", string.Join( ",", ids.Select( x => x ) ), page, limit );
			return endpoint;
		}

		//TODO GUARD-220 Convert to cursors. "page=" in the Url is getting depricated
		public static string CreateInventoryLevelsIdsEndpoint( string[] ids, int page, int limit )
		{
			// INFO : limit should be 250. Page start from 1. Max of inventory_item_ids is 50.
			var endpoint = string.Format( "?location_ids={0}&page={1}&limit={2}", string.Join( ",", ids.Select( x => x ) ), page, limit );
			return endpoint;
		}

		public static string CreateGetUserEndpoint( long userId )
		{
			var endpoint = string.Format( "{0}.json", userId );
			return endpoint;
		}

		public static string ConcatEndpoints( this string mainEndpoint, params string[] endpoints )
		{
			var result = new StringBuilder( mainEndpoint );

			foreach( var endpoint in endpoints )
				result.Append( endpoint.Replace( "?", "&" ) );

			return result.ToString();
		}
	}
}