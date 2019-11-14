namespace ShopifyAccess.Models.Configuration.Command
{
	public class ShopifyCommand
	{
		public static readonly ShopifyCommand Unknown = new ShopifyCommand( string.Empty );
		public static readonly ShopifyCommand GetAccessToken = new ShopifyCommand( "/admin/oauth/access_token" );
		public static readonly ShopifyCommand GetAllOrders = new ShopifyCommand( "/admin/orders.json" );
		public static readonly ShopifyCommand UpdateProductVariant = new ShopifyCommand( "/admin/variants/" ); // WARN: Obsolete!!! Please use UpdateInventoryLevels
		public static readonly ShopifyCommand UpdateInventoryLevels = new ShopifyCommand( "/admin/inventory_levels/set.json" );
		public static readonly ShopifyCommand GetProducts = new ShopifyCommand( "/admin/products.json" );
		public static readonly ShopifyCommand GetProductsCount = new ShopifyCommand( "/admin/products/count.json" );
		public static readonly ShopifyCommand GetInventoryLevels = new ShopifyCommand( "/admin/inventory_levels.json" );
		public static readonly ShopifyCommand GetOrdersCount = new ShopifyCommand( $"/admin/api/{ApiVersions.V2019_04}/orders/count.json" );
		public static readonly ShopifyCommand GetOrders = new ShopifyCommand( $"/admin/api/{ApiVersions.V2019_04}/orders.json" );
		public static readonly ShopifyCommand GetLocations = new ShopifyCommand( "/admin/locations.json" );
		public static readonly ShopifyCommand GetUsers = new ShopifyCommand( "/admin/users.json" );
		public static readonly ShopifyCommand GetUser = new ShopifyCommand( "/admin/users/" );

		private ShopifyCommand( string command )
		{
			this.Command = command;
		}

		public string Command{ get; private set; }
	}

	public struct ApiVersions
	{
		public static readonly string V2019_04 = "2019-04";
	}
}