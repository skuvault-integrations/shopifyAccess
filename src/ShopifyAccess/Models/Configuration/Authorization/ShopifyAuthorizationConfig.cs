using System;
using ShopifyAccess.Services;

namespace ShopifyAccess.Models.Configuration.Authorization
{
	public class ShopifyAuthorizationConfig : ShopifyShop
	{
		public string ApiKey { get; private set; }
		public string Secret { get; private set; }
		public Scopes Scopes { get; private set; }

		public ShopifyAuthorizationConfig( string apiKey, string secret, string shopName )
			: base( shopName )
		{
			if( string.IsNullOrWhiteSpace( apiKey ) )
			{
				throw new ArgumentException( "apiKey must not be null or whitespace", nameof(apiKey) );
			}

			if( string.IsNullOrWhiteSpace( secret ) )
			{
				throw new ArgumentException( "secret must not be null or whitespace", nameof(secret) );
			}

			this.ApiKey = apiKey;
			this.Secret = secret;
		}

		public ShopifyAuthorizationConfig( string apiKey, string secret, string shopName, Scopes scopes )
			: this( apiKey, secret, shopName )
		{
			this.Scopes = scopes ?? throw new ArgumentNullException( nameof(scopes), "scopes must not be null" );
		}

		//TODO GUARD-2753 Tech debt:
		//Problem: This method bypasses the ShopifyService and calls WebRequestServices directly to call the Shopify API.
		//Every other call to the Shopify API goes through ShopifyService (instantiated via ShopifyFactory). Plus, this class is not a service or a client.
		//It shouldn't call the API.
		//Solution: The RequestPermanentToken() is different from methods in ShopifyService. At this point in the authorization flow, we don't yet have
		//ShopifyCommandConfig with shopName, accessToken. Therefore, we probably want to create ShopifyAuthorizationService,
		//and move RequestPermanentToken() there from WebRequestServices. Should create IShopifyFactory.CreateAuthorizationService(),
		//and call it from where authorizationConfig.GetPermanentToken() is currently called in v1.
		public string GetPermanentToken( string code )
		{
			var webService = new WebRequestServices( this );
			return webService.RequestPermanentToken( code, Mark.Create );
		}
	}
}