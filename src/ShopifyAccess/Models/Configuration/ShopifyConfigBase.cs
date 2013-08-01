using CuttingEdge.Conditions;

namespace ShopifyAccess.Models.Configuration
{
	public class ShopifyConfigBase
	{
		public string Host { get; private set; }
		public string ShopName { get; private set; }

		protected ShopifyConfigBase( string shopName )
		{
			Condition.Requires( shopName, "shopName" ).IsNotNullOrWhiteSpace();

			this.Host = string.Format( "https://{0}.myshopify.com", shopName );
			this.ShopName = shopName;
		}
	}
}