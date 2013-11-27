namespace ShopifyAccess.Models.Configuration.Command
{
	internal class ShopifyOrderCommandEndpointName
	{
		public static readonly ShopifyOrderCommandEndpointName Unknown = new ShopifyOrderCommandEndpointName( string.Empty );
		public static readonly ShopifyOrderCommandEndpointName OrdersDateFrom = new ShopifyOrderCommandEndpointName( "created_at_min" );
		public static readonly ShopifyOrderCommandEndpointName OrdersDateTo = new ShopifyOrderCommandEndpointName( "created_at_max" );
		public static readonly ShopifyOrderCommandEndpointName OrderFulfillmentStatus = new ShopifyOrderCommandEndpointName( "fulfillment_status" );
		public static readonly ShopifyOrderCommandEndpointName OrdersDateUpdatedFrom = new ShopifyOrderCommandEndpointName( "updated_at_min" );
		public static readonly ShopifyOrderCommandEndpointName OrdersDateUpdatedTo = new ShopifyOrderCommandEndpointName( "updated_at_max" );
		public static readonly ShopifyOrderCommandEndpointName OrderStatus = new ShopifyOrderCommandEndpointName( "status" );
		public static readonly ShopifyOrderCommandEndpointName OrderFinancialStatus = new ShopifyOrderCommandEndpointName( "financial_status" );

		private ShopifyOrderCommandEndpointName( string name )
		{
			this.Name = name;
		}

		public string Name { get; private set; }
	}
}