using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using ServiceStack;
using ShopifyAccess.Misc;
using ShopifyAccess.Models;
using ShopifyAccess.Models.Configuration.Authorization;
using ShopifyAccess.Models.Configuration.Command;
using HttpHeaders = System.Net.Http.Headers.HttpHeaders;

namespace ShopifyAccess.Services
{
	internal sealed class WebRequestServices
	{
		private readonly ShopifyAuthorizationConfig _authorizationConfig;
		private readonly ShopifyCommandConfig _commandConfig;
		
		public HttpClient HttpClient { get; }
		private const int MaxHttpRequestTimeoutInMinutes = 30;

		#region Constructors
		public WebRequestServices( ShopifyAuthorizationConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._authorizationConfig = config;
			this.HttpClient = CreateHttpClient();
			this._commandConfig = new ShopifyCommandConfig( config.ShopName, "authorization" );
		}

		public WebRequestServices( ShopifyCommandConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._commandConfig = config;
			this.HttpClient = CreateHttpClient();
			var servicePoint = ServicePointManager.FindServicePoint( new Uri( this._commandConfig.Host ) );
			servicePoint.ConnectionLimit = 1000;
		}

		private HttpClient CreateHttpClient()
		{
			var httpClient = new HttpClient();
			httpClient.Timeout = TimeSpan.FromMinutes( MaxHttpRequestTimeoutInMinutes );
			httpClient.DefaultRequestHeaders.Remove( "X-Shopify-Access-Token" );
			httpClient.DefaultRequestHeaders.Add( "X-Shopify-Access-Token", this._commandConfig.AccessToken );
			return httpClient; 
		}
		#endregion

		#region Requests handling
		public T GetResponse< T >( ShopifyCommand command, string endpoint, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();
			
			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}

			var uri = this.CreateRequestUri( command, endpoint );
			this.LogGetRequest( uri, mark, timeout );

			return this.ParseException( mark, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					var response = await this.HttpClient.GetAsync( uri, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					var content = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					return this.ParseResponse< T >( content, response.Headers, uri, mark );
				}
			} ).Result;
		}

		public ResponsePage< T > GetResponsePage< T >( ShopifyCommand command, string endpoint, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}

			var uri = this.CreateRequestUri( command, endpoint );
			this.LogGetRequest( uri, mark, timeout );

			return this.ParseException( mark, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					var response = await this.HttpClient.GetAsync( uri, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					var content = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					return this.ParsePagedResponse< T >( content, response.Headers, uri, mark );
				}
			} ).Result;
		}

		public async Task< T > GetResponseAsync< T >( ShopifyCommand command, string endpoint, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}

			var uri = this.CreateRequestUri( command, endpoint );
			this.LogGetRequest( uri, mark, timeout );

			return await this.ParseExceptionAsync( mark, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					var response = await this.HttpClient.GetAsync( uri, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					return this.ParseResponse< T >( responseContent, response.Headers, uri, mark );
				}
			} ).ConfigureAwait( false );
		}

		public async Task< ResponsePage< T > > GetResponsePageAsync< T >( ShopifyCommand command, string endpoint, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}

			var uri = this.CreateRequestUri( command, endpoint );
			this.LogGetRequest( uri, mark, timeout );

			return await this.ParseExceptionAsync( mark, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					var response = await this.HttpClient.GetAsync( uri, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					var content = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					return this.ParsePagedResponse< T >( content, response.Headers, uri, mark );
				}
			} ).ConfigureAwait( false );
			
		}

		public void PutData( ShopifyCommand command, string endpoint, string jsonContent, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}

			var uri = this.CreateRequestUri( command, endpoint );
			this.LogUpdateRequest( uri, jsonContent, mark, timeout );

			this.ParseException( mark, () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					var content = new StringContent( jsonContent, Encoding.UTF8, "application/json" );
					var response = this.HttpClient.PutAsync( uri, content, linkedCancellationTokenSource.Token ).GetAwaiter().GetResult();
					this.LogUpdateResponse( uri, GetLimitFromHeader( response.Headers ), response.StatusCode, mark );
					return true;
				}
			} );
		}

		public async Task PutDataAsync( ShopifyCommand command, string endpoint, string jsonContent, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}

			var uri = this.CreateRequestUri( command, endpoint );
			this.LogUpdateRequest( uri, jsonContent, mark, timeout );

			await this.ParseExceptionAsync( mark, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					var content = new StringContent( jsonContent, Encoding.UTF8, "application/json" );
					var response = await this.HttpClient.PutAsync( uri, content, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					this.LogUpdateResponse( uri, GetLimitFromHeader( response.Headers ), response.StatusCode, mark );
					return Task.FromResult( true );
				}
			} ).ConfigureAwait( false );
		}

		public void PostData< T >( ShopifyCommand command, string jsonContent, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}

			var url = this.CreateRequestUri( command, endpoint: "" );
			this.LogUpdateRequest( url, jsonContent, mark, timeout );


			var content = new StringContent( jsonContent, Encoding.UTF8, "application/json" );	
			HttpResponseMessage response;
			using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
			{ 
				linkedCancellationTokenSource.CancelAfter( timeout );
				response = this.HttpClient.PostAsync( url, content, linkedCancellationTokenSource.Token ).GetAwaiter().GetResult();
			}

			this.ParseResponse< T >( response.Content.ReadAsStringAsync().Result, response.Headers, url, mark );
		}

		public async Task PostDataAsync< T >( ShopifyCommand command, string jsonContent, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}

			var url = this.CreateRequestUri( command, endpoint: "" );
			this.LogUpdateRequest( url, jsonContent, mark, timeout );


			var content = new StringContent( jsonContent, Encoding.UTF8, "application/json" );	
			HttpResponseMessage response;
			using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
			{ 
				linkedCancellationTokenSource.CancelAfter( timeout );
				response = await this.HttpClient.PostAsync( url, content, linkedCancellationTokenSource.Token );
			}

			this.ParseResponse< T >( await response.Content.ReadAsStringAsync(), response.Headers, url, mark );
		}

		public string RequestPermanentToken( string code, Mark mark )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var command = ShopifyCommand.GetAccessToken;
			var url = new Uri( string.Concat( this._authorizationConfig.Host, command.Command ) );
			var tokenRequestPostContent = string.Format( "client_id={0}&client_secret={1}&code={2}", this._authorizationConfig.ApiKey, this._authorizationConfig.Secret, code );
			var content = new StringContent( tokenRequestPostContent, Encoding.UTF8, "application/x-www-form-urlencoded" );	

			var response = this.HttpClient.PostAsync( url, content ).GetAwaiter().GetResult();

			var result = this.ParseResponse< TokenRequestResult >( response.Content.ReadAsStringAsync().Result, response.Headers, url, mark ).Token;

			return result;
		}
		#endregion

		#region Parsing response
		private T ParseResponse< T >( string content, HttpHeaders headers, Uri uri, Mark mark )
		{
			var limit = GetLimitFromHeader( headers );
			this.LogGetResponse( uri, limit, content, mark );

			return !string.IsNullOrEmpty( content ) ? content.FromJson< T >() : default( T );
		}

		private ResponsePage< T > ParsePagedResponse< T >( string content, HttpHeaders headers, Uri uri, Mark mark )
		{
			var limit = GetLimitFromHeader( headers );
			var nextPageLink = PagedResponseService.GetNextPageQueryStrFromHeader( headers );
			this.LogGetResponse( uri, limit, nextPageLink, content, mark );

			var result = !string.IsNullOrEmpty( content ) ? content.FromJson< T >() : default(T);

			return new ResponsePage< T > 
			{
				Response = result,
				NextPageQueryStr = nextPageLink
			};
		}

		private static string GetLimitFromHeader( HttpHeaders headers )
		{
			IEnumerable< string > limitHeader;
			headers.TryGetValues( "HTTP_X_SHOPIFY_SHOP_API_CALL_LIMIT", out limitHeader );
			return limitHeader?.FirstOrDefault() ?? string.Empty;
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

		private Uri CreateRequestUri( ShopifyCommand command, string endpoint )
		{
			return new Uri( string.Concat( this._commandConfig.Host, command.Command, endpoint ) );
		}
		#endregion

		#region Logging
		private void LogGetRequest( Uri requestUri, Mark mark, int timeout )
		{
			ShopifyLogger.Trace( mark, "GET request\tRequest: {0}", requestUri );
		}

		private void LogGetResponse( Uri requestUri, string limit, string jsonResponse, Mark mark )
		{
			ShopifyLogger.Trace( mark, "GET response\tRequest: {0}\tLimit: {1}\tResponse: {2}", requestUri, limit, jsonResponse );
		}

		private void LogGetResponse( Uri requestUri, string limit, string nextPage, string jsonResponse, Mark mark )
		{
			ShopifyLogger.Trace( mark, "GET response\tRequest: {0}\tLimit: {1}\tNext Page: {2}\tResponse: {3}", requestUri, limit, nextPage, jsonResponse );
		}

		private void LogUpdateRequest( Uri requestUri, string jsonContent, Mark mark, int timeout )
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