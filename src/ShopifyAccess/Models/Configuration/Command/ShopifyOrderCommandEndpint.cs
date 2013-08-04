namespace ShopifyAccess.Models.Configuration.Command
{
	internal class ShopifyOrderCommandEndpointName
	{
		public static readonly ShopifyOrderCommandEndpointName OrdersDateFrom = new ShopifyOrderCommandEndpointName( "created_at_min" );
		public static readonly ShopifyOrderCommandEndpointName OrdersDateTo = new ShopifyOrderCommandEndpointName( "created_at_max" );
		public static readonly ShopifyOrderCommandEndpointName FulfillmentStatus = new ShopifyOrderCommandEndpointName( "fulfillment_status" );

		private ShopifyOrderCommandEndpointName( string name )
		{
			this.Name = name;
		}

		public string Name { get; private set; }
	}
}