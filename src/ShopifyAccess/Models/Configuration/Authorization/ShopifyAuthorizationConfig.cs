using System;
using System.Web;
using CuttingEdge.Conditions;
using ShopifyAccess.Services;

namespace ShopifyAccess.Models.Configuration.Authorization
{
	public class ShopifyAuthorizationConfig : ShopifyConfigBase
	{
		public string ApiKey { get; private set; }
		public string Secret { get; private set; }
		public Scopes Scopes { get; private set; }
		public string RedirectUrl { get; private set; }

		public ShopifyAuthorizationConfig( string apiKey, string secret, string shopName )
			: base( shopName )
		{
			Condition.Requires( apiKey, "apiKey" ).IsNotNullOrWhiteSpace();
			Condition.Requires( apiKey, "apiKey" ).IsNotNullOrWhiteSpace();

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

		public string GetPermanentToken( string code )
		{
			var webService = new WebRequestServices( this );
			return webService.RequestPermanentToken( code );
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