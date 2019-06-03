using CuttingEdge.Conditions;

namespace ShopifyAccess.Models.Configuration.Command
{
	public class ShopifyCommandConfig : ShopifyConfigBase
	{
		public string AccessToken { get; private set; }
		public int RequestTimeoutMs { get; private set; }

		public ShopifyCommandConfig( string shopName, string accessToken, int requestTimeoutMs = 10 * 60 * 1000 )
			: base( shopName )
		{
			Condition.Requires( accessToken, "accessToken" ).IsNotNullOrWhiteSpace();

			this.AccessToken = accessToken;
			this.RequestTimeoutMs = requestTimeoutMs;
		}
	}
}