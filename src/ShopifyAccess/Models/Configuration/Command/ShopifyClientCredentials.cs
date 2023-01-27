using CuttingEdge.Conditions;

namespace ShopifyAccess.Models.Configuration.Command
{
	public class ShopifyClientCredentials : ShopifyShop
	{
		public string AccessToken { get; private set; }

		public ShopifyClientCredentials( string shopName, string accessToken )
			: base( shopName )
		{
			Condition.Requires( accessToken, "accessToken" ).IsNotNullOrWhiteSpace();

			this.AccessToken = accessToken;
		}
	}
}