using System;
using System.Web;
using CuttingEdge.Conditions;
using ShopifyAccess.Services;

namespace ShopifyAccess.Models.Core.Configuration
{
	public class ShopifyAuthorizationConfig : ShopifyConfigBase, IShopifyConfig
	{
		public string ApiKey { get; private set; }
		public string Sekret { get; private set; }
		public string ShopName { get; private set; }
		public Scopes Scopes { get; private set; }
		public string RedirectUrl { get; private set; }

		public ShopifyAuthorizationConfig( string apiKey, string sekret, string shopName )
			: base( shopName )
		{
			Condition.Requires( apiKey, "apiKey" ).IsNotNullOrWhiteSpace();
			Condition.Requires( apiKey, "apiKey" ).IsNotNullOrWhiteSpace();
			Condition.Requires( shopName, "shopName" ).IsNotNullOrWhiteSpace();

			this.ShopName = shopName;
			this.ApiKey = apiKey;
			this.Sekret = sekret;
		}

		public ShopifyAuthorizationConfig( string apiKey, string sekret, string shopName, Scopes scopes )
			: this( apiKey, sekret, shopName )
		{
			Condition.Requires( scopes, "scopes" ).IsNotNull();

			this.Scopes = scopes;
		}

		public ShopifyAuthorizationConfig( string apiKey, string sekret, string shopName, Scopes scopes, string redirectUrl )
			: this( apiKey, sekret, shopName, scopes )
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