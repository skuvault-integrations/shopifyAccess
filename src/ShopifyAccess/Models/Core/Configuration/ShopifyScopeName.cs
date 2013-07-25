namespace ShopifyAccess.Models.Core.Configuration
{
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

		private ShopifyScopeName( string name )
		{
			this.Name = name;
		}

		public string Name { get; private set; }
	}

	public enum ShopifyScopeAccessLevelEnum
	{
		Undefined,
		Read,
		Write
	}
}