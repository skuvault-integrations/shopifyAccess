namespace ShopifyAccess.Models.Configuration.Command
{
	public class ShopifyCommand
	{
		public static readonly ShopifyCommand Unknown = new ShopifyCommand( string.Empty );
		public static readonly ShopifyCommand GetAccessToken = new ShopifyCommand( "/admin/oauth/access_token" );
		public static readonly ShopifyCommand GetAllOrders = new ShopifyCommand( "/admin/orders.json" );
		public static readonly ShopifyCommand UpdateProductVariant = new ShopifyCommand( "/admin/variants/" );
		public static readonly ShopifyCommand GetProducts = new ShopifyCommand( "/admin/products.json" );
		public static readonly ShopifyCommand GetProductsCount = new ShopifyCommand( "/admin/products/count.json" );
		public static readonly ShopifyCommand GetOrdersCount = new ShopifyCommand( "/admin/orders/count.json" );
		public static readonly ShopifyCommand GetOrders = new ShopifyCommand( "/admin/orders.json" );
		public static readonly ShopifyCommand GetLocations = new ShopifyCommand( "/admin/locations.json" );

		private ShopifyCommand( string command )
		{
			this.Command = command;
		}

		public string Command{ get; private set; }
	}
}