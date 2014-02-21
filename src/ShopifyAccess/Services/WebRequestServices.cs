using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Netco.Logging;
using ServiceStack;
using ShopifyAccess.Models.Configuration.Authorization;
using ShopifyAccess.Models.Configuration.Command;

namespace ShopifyAccess.Services
{
	internal sealed class WebRequestServices
	{
		private readonly ShopifyAuthorizationConfig _authorizationConfig;
		private readonly ShopifyCommandConfig _commandConfig;

		#region Constructors
		public WebRequestServices( ShopifyAuthorizationConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._authorizationConfig = config;
		}

		public WebRequestServices( ShopifyCommandConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._commandConfig = config;
		}
		#endregion

		#region Requests handling
		public T GetResponse< T >( ShopifyCommand command, string endpoint )
		{
			T result;
			var request = this.CreateServiceGetRequest( command, endpoint );
			using( var response = request.GetResponse() )
				result = ParseResponse< T >( response );

			return result;
		}

		public async Task< T > GetResponseAsync< T >( ShopifyCommand command, string endpoint )
		{
			T result;
			var request = this.CreateServiceGetRequest( command, endpoint );
			using( var response = await request.GetResponseAsync() )
				result = ParseResponse< T >( response );

			return result;
		}

		public void PutData( ShopifyCommand command, string endpoint, string jsonContent )
		{
			Condition.Requires( this._commandConfig, "config" ).IsNotNull();

			var request = this.CreateServicePutRequest( command, endpoint, jsonContent );
			this.Log().Trace( "[shopify] PUT Params. ShopName: {0}. Data: {1}", this._commandConfig.ShopName, jsonContent );
			using( var response = ( HttpWebResponse )request.GetResponse() )
				this.LogUpdateInfo( endpoint, response.StatusCode, jsonContent );
		}

		public async Task PutDataAsync( ShopifyCommand command, string endpoint, string jsonContent )
		{
			Condition.Requires( this._commandConfig, "config" ).IsNotNull();

			var request = this.CreateServicePutRequest( command, endpoint, jsonContent );
			this.Log().Trace( "[shopify] PUT Params. ShopName: {0}. Data: {1}", this._commandConfig.ShopName, jsonContent );
			using( var response = await request.GetResponseAsync() )
				this.LogUpdateInfo( endpoint, ( ( HttpWebResponse )response ).StatusCode, jsonContent );
		}

		public string RequestPermanentToken( string code )
		{
			string result;
			var command = ShopifyCommand.GetAccessToken;
			var tokenRequestUrl = new Uri( string.Concat( this._authorizationConfig.Host, command.Command ) );
			var tokenRequestPostContent = string.Format( "client_id={0}&client_secret={1}&code={2}", this._authorizationConfig.ApiKey, this._authorizationConfig.Secret, code );
			var request = this.CreateServicePostRequest( tokenRequestUrl, tokenRequestPostContent );

			using( var response = request.GetResponse() )
				result = this.ParseResponse< TokenRequestResult >( response ).Token;

			return result;
		}

		private T ParseResponse< T >( WebResponse response )
		{
			var result = default( T );

			using( var stream = response.GetResponseStream() )
			{
				var reader = new StreamReader( stream );
				var jsonResponse = reader.ReadToEnd();

				this.Log().Trace( "[shopify]\tResponse\t{0} - {1}", response.ResponseUri, jsonResponse );

				if( !String.IsNullOrEmpty( jsonResponse ) )
					result = jsonResponse.FromJson< T >();
			}

			return result;
		}
		#endregion

		#region WebRequest configuration
		private HttpWebRequest CreateServicePutRequest( ShopifyCommand command, string endpoint, string content )
		{
			var uri = new Uri( string.Concat( this._commandConfig.Host, command.Command, endpoint ) );
			var request = ( HttpWebRequest )WebRequest.Create( uri );

			request.Method = WebRequestMethods.Http.Put;
			request.ContentType = "application/json";
			request.Headers.Add( "X-Shopify-Access-Token", this._commandConfig.AccessToken );

			using( var writer = new StreamWriter( request.GetRequestStream() ) )
				writer.Write( content );

			return request;
		}

		private HttpWebRequest CreateServicePostRequest( Uri uri, string content )
		{
			var request = ( HttpWebRequest )WebRequest.Create( uri );
			request.Method = WebRequestMethods.Http.Post;
			request.KeepAlive = true;
			request.Credentials = CredentialCache.DefaultCredentials;
			request.ContentType = "application/x-www-form-urlencoded";

			if( !string.IsNullOrEmpty( content ) )
			{
				using( var writer = new StreamWriter( request.GetRequestStream() ) )
					writer.Write( content );
			}

			return request;
		}

		private HttpWebRequest CreateServiceGetRequest( ShopifyCommand command, string endpoint )
		{
			var uri = new Uri( string.Concat( this._commandConfig.Host, command.Command, endpoint ) );
			var request = ( HttpWebRequest )WebRequest.Create( uri );

			request.Method = WebRequestMethods.Http.Get;
			request.Headers.Add( "X-Shopify-Access-Token", this._commandConfig.AccessToken );

			return request;
		}
		#endregion

		#region Logging
		private void LogUpdateInfo( string endpoint, HttpStatusCode statusCode, string jsonContent )
		{
			this.Log().Trace( "[shopify]\tPUT/POST call for the endpoint '{0}' has been completed with code '{1}'.\n{2}", endpoint, statusCode, jsonContent );
		}
		#endregion
	}
}