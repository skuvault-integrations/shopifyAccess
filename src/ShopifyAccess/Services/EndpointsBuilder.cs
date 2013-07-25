using System;
using ShopifyAccess.Models.Core.Configuration.Command;

namespace ShopifyAccess.Services
{
	internal class EndpointsBuilder
	{
		public string CreateOrdersDateRangeEndpoint( DateTime startDate, DateTime endDate )
		{
			var endpoint = string.Format( "?{0}={1}&{2}={3}",
				ShopifyOrderCommandEndpointName.OrdersDateFrom.Name, startDate.ToString( "o" ),
				ShopifyOrderCommandEndpointName.OrdersDateTo.Name, endDate.ToString( "o" ) );
			return endpoint;
		}

		public string CreateOrdersFulfillmentStatusEndpoint( ShopifyOrderFulfillmentStatus status )
		{
			var endpoint = string.Format( "?{0}={1}", ShopifyOrderCommandEndpointName.FulfillmentStatus.Name, status );
			return endpoint;
		}

		public string CreateProductVariantUpdateEndpoint( long variantId )
		{
			var endpoint = string.Format( "{0}.json", variantId );
			return endpoint;
		}
	}
}