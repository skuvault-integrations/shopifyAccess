using CuttingEdge.Conditions;

namespace ShopifyAccess.Models.Configuration.Command
{
	public class ShopifyCommandConfig : ShopifyConfigBase
	{
		private const int DefaultRequestTimeoutMs = 10 * 60 * 1000;	
		public string AccessToken { get; private set; }
		public int RequestTimeoutMs { get; private set; }

		public ShopifyCommandConfig( string shopName, string accessToken, int requestTimeoutMs = DefaultRequestTimeoutMs )
			: base( shopName )
		{
			Condition.Requires( accessToken, "accessToken" ).IsNotNullOrWhiteSpace();
			Condition.Requires( requestTimeoutMs, "requestTimeoutMs" ).IsGreaterThan( 0 );

			this.AccessToken = accessToken;
			this.RequestTimeoutMs = requestTimeoutMs;
		}
	}
}