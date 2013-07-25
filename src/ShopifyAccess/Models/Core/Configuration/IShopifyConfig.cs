using System;

namespace ShopifyAccess.Models.Core.Configuration
{
	public interface IShopifyConfig
	{
		Uri GetAuthenticationUri();
		string GetPermanentToken( string code );
	}
}