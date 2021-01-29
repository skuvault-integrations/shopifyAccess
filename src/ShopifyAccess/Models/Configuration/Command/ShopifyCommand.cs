namespace ShopifyAccess.Models.Configuration.Command
{
	public class ShopifyCommand
	{
		private const string VersionSpecificBaseUrl = "/admin/api/{0}";

		public static readonly ShopifyCommand Unknown = new ShopifyCommand( string.Empty );
		public static readonly ShopifyCommand GetAccessToken = new ShopifyCommand( "/admin/oauth/access_token" );
		public static readonly ShopifyCommand GetAllOrders = new ShopifyCommand( "/admin/orders.json" );
		public static readonly ShopifyCommand UpdateProductVariant = new ShopifyCommand( "/admin/variants/" ); // WARN: Obsolete!!! Please use UpdateInventoryLevels
		public static readonly ShopifyCommand UpdateInventoryLevels = new ShopifyCommand( "/inventory_levels/set.json", ApiVersion.V2019_10 );
		public static readonly ShopifyCommand GetProducts = new ShopifyCommand( "/products.json", ApiVersion.V2019_10 );
		public static readonly ShopifyCommand GetProductsCount = new ShopifyCommand( "/products/count.json", ApiVersion.V2019_10 );
		public static readonly ShopifyCommand GetInventoryLevels = new ShopifyCommand( "/inventory_levels.json", ApiVersion.V2019_10 );
		public static readonly ShopifyCommand GetOrdersCount = new ShopifyCommand( "/orders/count.json", ApiVersion.V2019_10 );
		public static readonly ShopifyCommand GetOrders = new ShopifyCommand( "/orders.json", ApiVersion.V2019_10 );
		public static readonly ShopifyCommand GetLocations = new ShopifyCommand( "/locations.json", ApiVersion.V2019_10 );
		public static readonly ShopifyCommand GetUsers = new ShopifyCommand( "/users.json", ApiVersion.V2019_10 );
		public static readonly ShopifyCommand GetUser = new ShopifyCommand( "/admin/users/" );

		/// <summary>
		/// Create Shopify command using the oldest supported api version
		/// </summary>
		/// <param name="command"></param>
		private ShopifyCommand( string command )
		{
			this.Command = command;
		}

		/// <summary>
		/// Create Shopify command using a specific api version
		/// </summary>
		/// <param name="commandUrl"></param>
		/// <param name="apiVersion"></param>
		private ShopifyCommand( string commandUrl, string apiVersion )
		{
			this.Command = string.Format( VersionSpecificBaseUrl, apiVersion ) + commandUrl;
		}

		public string Command{ get; private set; }
	}

	public struct ApiVersion
	{
		public const string V2019_04 = "2019-04";
		public const string V2019_07 = "2019-07";
		public const string V2019_10 = "2019-10";
	}
}