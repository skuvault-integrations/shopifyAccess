using System;

namespace ShopifyAccess.Models.Configuration
{
	public class ShopifyShop
	{
		public string Host { get; private set; }
		public string ShopName { get; private set; }

		protected ShopifyShop( string shopName )
		{
			if( string.IsNullOrWhiteSpace( shopName ) )
			{
				throw new ArgumentException( "shopName must not be null or whitespace", nameof(shopName) );
			}

			this.Host = string.Format( "https://{0}.myshopify.com", shopName );
			this.ShopName = shopName;
		}
	}
}