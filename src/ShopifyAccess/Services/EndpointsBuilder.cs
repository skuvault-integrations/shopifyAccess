using System;
using System.Text;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Order;

namespace ShopifyAccess.Services
{
	internal static class EndpointsBuilder
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