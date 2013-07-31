namespace ShopifyAccess.Models.Configuration
{
	public class ShopifyConfigBase
	{
		public string Host { get; private set; }

		protected ShopifyConfigBase( string shopName )
		{
			this.Host = string.Format( "https://{0}.myshopify.com", shopName );
		}
	}
}