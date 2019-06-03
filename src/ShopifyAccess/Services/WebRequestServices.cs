using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using ServiceStack;
using ShopifyAccess.Misc;
using ShopifyAccess.Models;
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
			this._commandConfig = new ShopifyCommandConfig( config.ShopName, "authorization" );
		}

		public WebRequestServices( ShopifyCommandConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._commandConfig = config;
		}
		#endregion

		#region Requests handling
		public T GetResponse< T >( ShopifyCommand command, string endpoint, Mark mark )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var request = this.CreateServiceGetRequest( command, endpoint );
			this.LogGetRequest( request.RequestUri, mark );

			return this.ParseException( mark, () =>
			{
				T result;
				using( var response = request.GetResponse() )
					result = this.ParseResponse< T >( response, mark );
				return result;
			} );
		}

		public async Task< T > GetResponseAsync< T >( ShopifyCommand command, string endpoint, Mark mark )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var request = this.CreateServiceGetRequest( command, endpoint );
			this.LogGetRequest( request.RequestUri, mark );

			return await this.ParseExceptionAsync( mark, async () =>
			{
				T result;
				using( var response = await request.GetResponseAsync() )
					result = this.ParseResponse< T >( response, mark );
				return result;
			} );
		}

		public void PutData( ShopifyCommand command, string endpoint, string jsonContent, Mark mark )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var request = this.CreateServicePutRequest( command, endpoint, jsonContent );
			this.LogUpdateRequest( request.RequestUri, jsonContent, mark );

			this.ParseException( mark, () =>
			{
				using( var response = ( HttpWebResponse )request.GetResponse() )
					this.LogUpdateResponse( request.RequestUri, this.GetLimitFromHeader( response ), response.StatusCode, mark );
				return true;
			} );
		}

		public async Task PutDataAsync( ShopifyCommand command, string endpoint, string jsonContent, Mark mark )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var request = this.CreateServicePutRequest( command, endpoint, jsonContent );
			this.LogUpdateRequest( request.RequestUri, jsonContent, mark );

			await this.ParseExceptionAsync( mark, async () =>
			{
				using( var response = await request.GetResponseAsync() )
					this.LogUpdateResponse( request.RequestUri, this.GetLimitFromHeader( response ), ( ( HttpWebResponse )response ).StatusCode, mark );
				return Task.FromResult( true );
			} );
		}

		public T PostData< T >( ShopifyCommand command, string jsonContent, Mark mark )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var url = new Uri( string.Concat( this._commandConfig.Host, command.Command ) );

			var request = this.CreateServicePostRequest( url, this._commandConfig.AccessToken, jsonContent );
			this.LogUpdateRequest( request.RequestUri, jsonContent, mark );

			T result;

			using( var response = request.GetResponse() )
			{
				result = this.ParseResponse< T >( response, mark );
			}

			return default(T);
		}

		public async Task< T > PostDataAsync< T >( ShopifyCommand command, string jsonContent, Mark mark )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var url = new Uri( string.Concat( this._commandConfig.Host, command.Command ) );

			var request = this.CreateServicePostRequest( url, this._commandConfig.AccessToken, jsonContent );
			this.LogUpdateRequest( request.RequestUri, jsonContent, mark );

			T result;

			using( var response = await request.GetResponseAsync() )
			{
				result = this.ParseResponse< T >( response, mark );
			}

			return default(T);
		}

		public string RequestPermanentToken( string code, Mark mark )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			string result;
			var command = ShopifyCommand.GetAccessToken;
			var tokenRequestUrl = new Uri( string.Concat( this._authorizationConfig.Host, command.Command ) );
			var tokenRequestPostContent = string.Format( "client_id={0}&client_secret={1}&code={2}", this._authorizationConfig.ApiKey, this._authorizationConfig.Secret, code );
			var request = this.CreateServicePostRequest( tokenRequestUrl, tokenRequestPostContent );

			using( var response = request.GetResponse() )
				result = this.ParseResponse< TokenRequestResult >( response, mark ).Token;

			return result;
		}
		#endregion

		#region Parsing response
		private T ParseResponse< T >( WebResponse response, Mark mark )
		{
			var result = default(T);

			using( var stream = response.GetResponseStream() )
			using( var reader = new StreamReader( stream ) )
			{
				var jsonResponse = reader.ReadToEnd();

				var limit = this.GetLimitFromHeader( response );
				this.LogGetResponse( response.ResponseUri, limit, jsonResponse, mark );

				if( !string.IsNullOrEmpty( jsonResponse ) )
					result = jsonResponse.FromJson< T >();
			}

			return result;
		}

		private string GetLimitFromHeader( WebResponse response )
		{
			var limitMass = response.Headers.GetValues( "HTTP_X_SHOPIFY_SHOP_API_CALL_LIMIT" );
			var limit = limitMass != null && limitMass.Length > 0 ? limitMass[ 0 ] : string.Empty;
			return limit;
		}

		private T ParseException< T >( Mark mark, Func< T > body )
		{
			try
			{
				return body();
			}
			catch( WebException ex )
			{
				throw this.HandleException( ex, mark );
			}
		}

		private async Task< T > ParseExceptionAsync< T >( Mark mark, Func< Task< T > > body )
		{
			try
			{
				return await body();
			}
			catch( WebException ex )
			{
				throw this.HandleException( ex, mark );
			}
		}

		private WebException HandleException( WebException ex, Mark mark )
		{
			if( ex.Response == null || ex.Status != WebExceptionStatus.ProtocolError ||
			    ex.Response.ContentType == null || ex.Response.ContentType.Contains( "text/html" ) )
			{
				this.LogException( ex, mark );
				return ex;
			}

			var httpResponse = ( HttpWebResponse )ex.Response;

			using( var stream = httpResponse.GetResponseStream() )
			using( var reader = new StreamReader( stream ) )
			{
				var jsonResponse = reader.ReadToEnd();
				this.LogException( ex, httpResponse, jsonResponse, mark );
				return ex;
			}
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

		private HttpWebRequest CreateServicePostRequest( Uri uri, string token, string content )
		{
			var request = ( HttpWebRequest )WebRequest.Create( uri );
			request.Method = WebRequestMethods.Http.Post;
			request.KeepAlive = true;
			request.ContentType = "application/json";
			request.Headers.Add( "X-Shopify-Access-Token", token );

			if( !string.IsNullOrEmpty( content ) )
				using( var writer = new StreamWriter( request.GetRequestStream() ) )
				{
					writer.Write( content );
				}

			return request;
		}

		private HttpWebRequest CreateServiceGetRequest( ShopifyCommand command, string endpoint )
		{
			var uri = new Uri( string.Concat( this._commandConfig.Host, command.Command, endpoint ) );
			var servicePoint = ServicePointManager.FindServicePoint( new Uri( this._commandConfig.Host ) );
			servicePoint.ConnectionLimit = 1000;

			var request = ( HttpWebRequest )WebRequest.Create( uri );

			request.Method = WebRequestMethods.Http.Get;
			request.Headers.Add( "X-Shopify-Access-Token", this._commandConfig.AccessToken );
			request.Timeout = 60 * 1000 * 10;
			request.KeepAlive = false;
			request.ProtocolVersion = HttpVersion.Version10;
			request.ServicePoint.Expect100Continue = false;

			return request;
		}
		#endregion

		#region Logging
		private void LogGetRequest( Uri requestUri, Mark mark )
		{
			ShopifyLogger.Trace( mark, "GET request\tRequest: {0}", requestUri );
		}

		private void LogGetResponse( Uri requestUri, string limit, string jsonResponse, Mark mark )
		{
			ShopifyLogger.Trace( mark, "GET response\tRequest: {0}\tLimit: {1}\tResponse: {2}", requestUri, limit, jsonResponse );
		}

		private void LogUpdateRequest( Uri requestUri, string jsonContent, Mark mark )
		{
			ShopifyLogger.Trace( mark, "PUT request\tRequest: {0}\tData: {1}", requestUri, jsonContent );
		}

		private void LogUpdateResponse( Uri requestUri, string limit, HttpStatusCode statusCode, Mark mark )
		{
			ShopifyLogger.Trace( mark, "PUT/POST response\tRequest: {0}\tLimit: {1}\tStatusCode: {2}", requestUri, limit, statusCode );
		}

		private void LogException( WebException ex, Mark mark )
		{
			ShopifyLogger.Trace( ex, mark, "Failed response\tShopName: {0}\tMessage: {1}\tStatus: {2}", this._commandConfig.ShopName, ex.Message, ex.Status );
		}

		private void LogException( WebException ex, HttpWebResponse response, string jsonResponse, Mark mark )
		{
			ShopifyLogger.Trace( ex, mark, "Failed response\tRequest: {0}\tMessage: {1}\tStatus: {2}\tJsonResponse: {3}",
				response.ResponseUri, ex.Message, response.StatusCode, jsonResponse );
		}
		#endregion
	}
}