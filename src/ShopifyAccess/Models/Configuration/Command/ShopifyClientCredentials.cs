using System;

namespace ShopifyAccess.Models.Configuration.Command
{
	public class ShopifyClientCredentials : ShopifyShop
	{
		public string AccessToken { get; private set; }

		public ShopifyClientCredentials( string shopName, string accessToken )
			: base( shopName )
		{
			if( string.IsNullOrWhiteSpace( accessToken ) )
			{
				throw new ArgumentException( "accessToken must not be null or whitespace", nameof(accessToken) );
			}


			this.AccessToken = accessToken;
		}
	}
}