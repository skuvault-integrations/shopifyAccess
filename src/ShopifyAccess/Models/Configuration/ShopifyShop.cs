using CuttingEdge.Conditions;

namespace ShopifyAccess.Models.Configuration
{
	public class ShopifyShop
	{
		public string Host { get; private set; }
		public string ShopName { get; private set; }

		protected ShopifyShop( string shopName )
		{
			Condition.Requires( shopName, "shopName" ).IsNotNullOrWhiteSpace();

			this.Host = string.Format( "https://{0}.myshopify.com", shopName );
			this.ShopName = shopName;
		}
	}
}