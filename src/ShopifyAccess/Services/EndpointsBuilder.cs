using System;
using System.Linq;
using System.Text;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Order;

namespace ShopifyAccess.Services
{
	public static class EndpointsBuilder
	{
		public static string CreateUpdatedOrdersEndpoint( ShopifyOrderStatus status, DateTime startDate, DateTime endDate )
		{
			var endpoint = string.Format( "?{0}={1}&{2}={3}&{4}={5}",
				ShopifyOrderCommandEndpointName.OrderStatus.Name, status,
				ShopifyOrderCommandEndpointName.OrdersDateUpdatedFrom.Name, DateTime.SpecifyKind( startDate, DateTimeKind.Utc ).ToString( "o" ),
				ShopifyOrderCommandEndpointName.OrdersDateUpdatedTo.Name, DateTime.SpecifyKind( endDate, DateTimeKind.Utc ).ToString( "o" ));
			return endpoint;
		}

		public static string CreateGetEndpoint( ShopifyCommandEndpointConfig config )
		{
			var endpoint = string.Format( "?{0}={1}",
				ShopifyCommandEndpointName.Limit.Name, config.Limit
				);
			return endpoint;
		}

		public static string CreateInventoryLevelsIdsEndpoint( long[] ids, int limit )
		{
			// INFO : Max of inventory_item_ids is 50.
			var endpoint = string.Format( "?inventory_item_ids={0}&limit={1}", string.Join( ",", ids.Select( x => x ) ), limit );
			return endpoint;
		}

		public static string CreateInventoryLevelsIdsEndpoint( string[] ids, int limit )
		{
			// INFO : Max of inventory_item_ids is 50.
			var endpoint = string.Format( "?location_ids={0}&limit={1}", string.Join( ",", ids.Select( x => x ) ), limit );
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