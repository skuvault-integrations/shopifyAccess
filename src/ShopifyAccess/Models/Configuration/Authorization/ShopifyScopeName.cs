namespace ShopifyAccess.Models.Configuration.Authorization
{
	/// <summary>
	/// Authorization scopes our syncs need. NOTE: In v1 we pre-pend "read" or "write" at the beginning. For example, "read_products".
	/// All scopes - <see href="https://shopify.dev/docs/api/admin-rest/usage/access-scopes"/>
	/// </summary>
	public class ShopifyScopeName
	{
		public static readonly ShopifyScopeName Content = new ShopifyScopeName( "_content" );
		public static readonly ShopifyScopeName Themes = new ShopifyScopeName( "_themes" );
		public static readonly ShopifyScopeName Products = new ShopifyScopeName( "_products" );
		public static readonly ShopifyScopeName Customers = new ShopifyScopeName( "_customers" );
		public static readonly ShopifyScopeName Orders = new ShopifyScopeName( "_orders" );
		public static readonly ShopifyScopeName ScriptTags = new ShopifyScopeName( "_script_tags" );
		public static readonly ShopifyScopeName Fulfillments = new ShopifyScopeName( "_fulfillments" );
		public static readonly ShopifyScopeName Shipping = new ShopifyScopeName( "_shipping" );
		public static readonly ShopifyScopeName Inventory = new ShopifyScopeName( "_inventory" );
		public static readonly ShopifyScopeName AllOrders = new ShopifyScopeName( "_all_orders" );
		public static readonly ShopifyScopeName Locations = new ShopifyScopeName( "_locations" );
		
		private ShopifyScopeName( string name )
		{
			this.Name = name;
		}

		public string Name { get; private set; }
	}

	public enum ShopifyScopeAccessLevel
	{
		Undefined,
		Read,
		Write
	}
}