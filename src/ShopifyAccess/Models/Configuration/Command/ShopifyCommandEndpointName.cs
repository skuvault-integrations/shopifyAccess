namespace ShopifyAccess.Models.Configuration.Command
{
	internal class ShopifyCommandEndpointName
	{
		public static readonly ShopifyCommandEndpointName Unknown = new ShopifyCommandEndpointName( string.Empty );
		public static readonly ShopifyCommandEndpointName Limit = new ShopifyCommandEndpointName( "limit" );
		public static readonly ShopifyCommandEndpointName Page = new ShopifyCommandEndpointName( "page" );
		public static readonly ShopifyCommandEndpointName Fields = new ShopifyCommandEndpointName( "fields" );

		private ShopifyCommandEndpointName( string name )
		{
			this.Name = name;
		}

		public string Name { get; private set; }
	}
}