namespace ShopifyAccess.Models.Configuration.Command
{
	public class ShopifyCommand
	{
		private const string VersionSpecificBaseUrl = "/admin/api/{0}";

		//This command is here while others are in ShopifyCommandFactory since this one is not versioned
		public static ShopifyCommand GetAccessToken => new ShopifyCommand( "/admin/oauth/access_token" );

		/// <summary>
		/// Create Shopify command using the oldest supported api version
		/// </summary>
		/// <param name="command"></param>
		internal ShopifyCommand( string command )
		{
			this.Command = command;
		}

		/// <summary>
		/// Create Shopify command using a specific api version
		/// </summary>
		/// <param name="commandUrl"></param>
		/// <param name="apiVersion"></param>
		internal ShopifyCommand( string commandUrl, ShopifyApiVersion apiVersion )
		{
			this.Command = string.Format( VersionSpecificBaseUrl, apiVersion.ToString() ) + commandUrl;
		}

		public string Command{ get; private set; }
	}
}