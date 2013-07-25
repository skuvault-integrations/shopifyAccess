namespace ShopifyAccess.Models.Core.Configuration.Command
{
	public class ShopifyCommand
	{
		public static readonly ShopifyCommand GetAccessToken = new ShopifyCommand( "/admin/oauth/access_token" );
		public static readonly ShopifyCommand GetAllOrders = new ShopifyCommand( "/admin/orders.json" );

		private ShopifyCommand( string command )
		{
			this.Command = command;
		}

		public string Command { get; private set; }
	}
}