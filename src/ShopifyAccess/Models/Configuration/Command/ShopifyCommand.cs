namespace ShopifyAccess.Models.Configuration.Command
{
	public class ShopifyCommand
	{
		public static readonly ShopifyCommand Unknown = new ShopifyCommand( string.Empty );
		public static readonly ShopifyCommand GetAccessToken = new ShopifyCommand( "/admin/oauth/access_token" );
		public static readonly ShopifyCommand GetAllOrders = new ShopifyCommand( "/admin/orders.json" );
		public static readonly ShopifyCommand UpdateProductVariant = new ShopifyCommand( "/admin/variants/" );

		private ShopifyCommand( string command )
		{
			this.Command = command;
		}

		public string Command { get; private set; }
	}
}