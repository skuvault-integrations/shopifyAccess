using CuttingEdge.Conditions;

namespace ShopifyAccess.Models.Core.Configuration.Command
{
	public class ShopifyCommandConfig : ShopifyConfigBase
	{
		public string AccessToken { get; private set; }

		public ShopifyCommandConfig( string shopName, string accessToken )
			: base( shopName )
		{
			Condition.Requires( accessToken, "accessToken" ).IsNotNullOrWhiteSpace();

			this.AccessToken = accessToken;
		}
	}
}