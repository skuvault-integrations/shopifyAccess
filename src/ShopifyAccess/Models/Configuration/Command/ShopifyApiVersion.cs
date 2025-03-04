namespace ShopifyAccess.Models.Configuration.Command
{
	/// <summary>
	/// Shopify releases a new API version at the beginning of each quarter and retires the version that's has just turned
	/// one hear old, per https://shopify.dev/api/usage/versioning#release-schedule. If a retired API version is requested
	/// by an API call, Shopify uses the oldest currently available API version ("effective" version noted in Http response)
	/// </summary>
	public sealed class ShopifyApiVersion
	{
		private readonly string _versionCode;
		
		public static readonly ShopifyApiVersion V2024_04 = new ShopifyApiVersion( "2024-04" );
		public static readonly ShopifyApiVersion V2024_07 = new ShopifyApiVersion( "2024-07" );
		public static readonly ShopifyApiVersion V2024_10 = new ShopifyApiVersion( "2024-10" );
		public static readonly ShopifyApiVersion V2025_01 = new ShopifyApiVersion( "2025-01" );

		private ShopifyApiVersion( string versionCode )
		{
			this._versionCode = versionCode;
		}

		public override string ToString()
		{
			return this._versionCode;
		}
	}
}