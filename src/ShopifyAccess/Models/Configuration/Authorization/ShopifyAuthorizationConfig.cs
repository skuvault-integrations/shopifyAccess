using System;
using System.Web;
using CuttingEdge.Conditions;
using ShopifyAccess.Services;

namespace ShopifyAccess.Models.Configuration.Authorization
{
	public class ShopifyAuthorizationConfig : ShopifyShop
	{
		public string ApiKey { get; private set; }
		public string Secret { get; private set; }
		public Scopes Scopes { get; private set; }
		public string RedirectUrl { get; private set; }

		public ShopifyAuthorizationConfig( string apiKey, string secret, string shopName )
			: base( shopName )
		{
			Condition.Requires( apiKey, "apiKey" ).IsNotNullOrWhiteSpace();
			Condition.Requires( secret, "secret" ).IsNotNullOrWhiteSpace();

			this.ApiKey = apiKey;
			this.Secret = secret;
		}

		public ShopifyAuthorizationConfig( string apiKey, string secret, string shopName, Scopes scopes )
			: this( apiKey, secret, shopName )
		{
			Condition.Requires( scopes, "scopes" ).IsNotNull();

			this.Scopes = scopes;
		}

		public ShopifyAuthorizationConfig( string apiKey, string secret, string shopName, Scopes scopes, string redirectUrl )
			: this( apiKey, secret, shopName, scopes )
		{
			Condition.Requires( redirectUrl, "redirectUrl" ).IsNotNullOrWhiteSpace();

			this.RedirectUrl = redirectUrl;
		}

		public Uri GetAuthenticationUri()
		{
			var uri = new Uri( string.Format( "https://{0}.myshopify.com/admin/oauth/authorize?client_id={1}&scope={2}{3}",
				this.ShopName,
				this.ApiKey,
				this.Scopes.ScopesString,
				this.GetRedirectUrl() ) );

			return uri;
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

		private string GetRedirectUrl()
		{
			var result = string.Empty;

			if( !string.IsNullOrEmpty( this.RedirectUrl ) )
				result = string.Format( "&redirect_uri={0}", HttpUtility.UrlEncode( this.RedirectUrl ) );

			return result;
		}
	}
}