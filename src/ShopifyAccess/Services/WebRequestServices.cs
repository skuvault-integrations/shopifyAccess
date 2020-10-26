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
		
		public HttpClient HttpClient { get; private set; }
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

			var uri = this.CreateRequestUri( command, endpoint );
			ShopifyLogger.LogGetRequest( uri, mark, timeout );
			
			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}

			return this.ParseException( mark, timeout, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					var response = await this.HttpClient.GetAsync( uri, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					var content = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					return ParseResponse< T >( content, response.Headers, uri, mark, timeout );
				}
			} ).Result;
		}

		public ResponsePage< T > GetResponsePage< T >( ShopifyCommand command, string endpoint, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var uri = this.CreateRequestUri( command, endpoint );
			ShopifyLogger.LogGetRequest( uri, mark, timeout );

			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}

			return this.ParseException( mark, timeout, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					var response = await this.HttpClient.GetAsync( uri, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					var content = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					return ParsePagedResponse< T >( content, response.Headers, uri, mark, timeout );
				}
			} ).Result;
		}

		public async Task< T > GetResponseAsync< T >( ShopifyCommand command, string endpoint, CancellationToken token, Mark mark, int timeout, [ CallerMemberName ] string methodName = "" )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}

			var uri = this.CreateRequestUri( command, endpoint );
			ShopifyLogger.LogGetRequest( uri, mark, timeout );

			return await this.ParseExceptionAsync( mark, timeout, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					var response = await this.HttpClient.GetAsync( uri, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					return ParseResponse< T >( responseContent, response.Headers, uri, mark, timeout );
				}
			} ).ConfigureAwait( false );
		}

		public async Task< ResponsePage< T > > GetResponsePageAsync< T >( ShopifyCommand command, string endpoint, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var uri = this.CreateRequestUri( command, endpoint );
			ShopifyLogger.LogGetRequest( uri, mark, timeout );

			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}

			return await this.ParseExceptionAsync( mark, timeout, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					var response = await this.HttpClient.GetAsync( uri, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					var content = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					return ParsePagedResponse< T >( content, response.Headers, uri, mark, timeout );
				}
			} ).ConfigureAwait( false );
			
		}

		public void PutData( ShopifyCommand command, string endpoint, string jsonContent, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var uri = this.CreateRequestUri( command, endpoint );
			ShopifyLogger.LogUpdateRequest( uri, jsonContent, mark, timeout );

			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}

			this.ParseException( mark, timeout, () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					var content = new StringContent( jsonContent, Encoding.UTF8, "application/json" );
					var response = this.HttpClient.PutAsync( uri, content, linkedCancellationTokenSource.Token ).GetAwaiter().GetResult();
					ShopifyLogger.LogUpdateResponse( uri, GetLimitFromHeader( response.Headers ), response.StatusCode, mark, timeout );
					return true;
				}
			} );
		}

		public async Task PutDataAsync( ShopifyCommand command, string endpoint, string jsonContent, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var uri = this.CreateRequestUri( command, endpoint );
			ShopifyLogger.LogUpdateRequest( uri, jsonContent, mark, timeout );

			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}

			await this.ParseExceptionAsync( mark, timeout, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					var content = new StringContent( jsonContent, Encoding.UTF8, "application/json" );
					var response = await this.HttpClient.PutAsync( uri, content, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					ShopifyLogger.LogUpdateResponse( uri, GetLimitFromHeader( response.Headers ), response.StatusCode, mark, timeout );
					return Task.FromResult( true );
				}
			} ).ConfigureAwait( false );
		}

		public void PostData< T >( ShopifyCommand command, string jsonContent, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var url = this.CreateRequestUri( command, endpoint: "" );
			ShopifyLogger.LogUpdateRequest( url, jsonContent, mark, timeout );

			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}


			var content = new StringContent( jsonContent, Encoding.UTF8, "application/json" );	
			HttpResponseMessage response;
			using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
			{ 
				linkedCancellationTokenSource.CancelAfter( timeout );
				response = this.HttpClient.PostAsync( url, content, linkedCancellationTokenSource.Token ).GetAwaiter().GetResult();
			}

			ParseResponse< T >( response.Content.ReadAsStringAsync().Result, response.Headers, url, mark, timeout );
		}

		public async Task PostDataAsync< T >( ShopifyCommand command, string jsonContent, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var url = this.CreateRequestUri( command, endpoint: "" );
			ShopifyLogger.LogUpdateRequest( url, jsonContent, mark, timeout );

			if( token.IsCancellationRequested )
			{
				throw this.HandleException( new WebException( "Task was cancelled" ), mark );
			}


			var content = new StringContent( jsonContent, Encoding.UTF8, "application/json" );	
			HttpResponseMessage response;
			using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
			{ 
				linkedCancellationTokenSource.CancelAfter( timeout );
				response = await this.HttpClient.PostAsync( url, content, linkedCancellationTokenSource.Token );
			}

			ParseResponse< T >( await response.Content.ReadAsStringAsync(), response.Headers, url, mark, timeout );
		}

		public string RequestPermanentToken( string code, Mark mark )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var command = ShopifyCommand.GetAccessToken;
			var url = new Uri( string.Concat( this._authorizationConfig.Host, command.Command ) );
			var tokenRequestPostContent = string.Format( "client_id={0}&client_secret={1}&code={2}", this._authorizationConfig.ApiKey, this._authorizationConfig.Secret, code );
			var content = new StringContent( tokenRequestPostContent, Encoding.UTF8, "application/x-www-form-urlencoded" );	

			var response = this.HttpClient.PostAsync( url, content ).GetAwaiter().GetResult();

			var result = ParseResponse< TokenRequestResult >( response.Content.ReadAsStringAsync().Result, response.Headers, url, mark ).Token;

			return result;
		}
		#endregion

		#region Parsing response
		private static T ParseResponse< T >( string content, HttpHeaders headers, Uri uri, Mark mark )
		{
			return ParseResponse< T >( content, headers, uri, mark, TimeSpan.FromMinutes( MaxHttpRequestTimeoutInMinutes ).Milliseconds );
		}

		private static T ParseResponse< T >( string content, HttpHeaders headers, Uri uri, Mark mark, int timeout )
		{
			var limit = GetLimitFromHeader( headers );
			ShopifyLogger.LogGetResponse( uri, limit, content, mark, timeout );

			return !string.IsNullOrEmpty( content ) ? content.FromJson< T >() : default( T );
		}

		private static ResponsePage< T > ParsePagedResponse< T >( string content, HttpHeaders headers, Uri uri, Mark mark, int timeout )
		{
			var limit = GetLimitFromHeader( headers );
			var nextPageLink = PagedResponseService.GetNextPageQueryStrFromHeader( headers );
			ShopifyLogger.LogGetResponse( uri, limit, nextPageLink, content, mark, timeout );

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

		private T ParseException< T >( Mark mark, int timeout, Func< T > body )
		{
			try
			{
				return body();
			}
			catch( WebException ex )
			{
				throw this.HandleException( ex, mark );
			}
			catch( TaskCanceledException )
			{
				ShopifyLogger.LogTimeoutException( mark, this._commandConfig.ShopName, timeout );
				throw;
			}
		}

		private async Task< T > ParseExceptionAsync< T >( Mark mark, int timeout, Func< Task< T > > body )
		{
			try
			{
				return await body();
			}
			catch( WebException ex )
			{
				throw this.HandleException( ex, mark );
			}
			catch( TaskCanceledException )
			{
				ShopifyLogger.LogTimeoutException( mark, this._commandConfig.ShopName, timeout );
				throw;
			}
		}

		private WebException HandleException( WebException ex, Mark mark )
		{
			if( ex.Response == null || ex.Status != WebExceptionStatus.ProtocolError ||
			    ex.Response.ContentType == null || ex.Response.ContentType.Contains( "text/html" ) )
			{
				ShopifyLogger.LogException( ex, mark, this._commandConfig.ShopName );
				return ex;
			}

			var httpResponse = ( HttpWebResponse )ex.Response;

			using( var stream = httpResponse.GetResponseStream() )
			using( var reader = new StreamReader( stream ) )
			{
				var jsonResponse = reader.ReadToEnd();
				ShopifyLogger.LogException( ex, httpResponse, jsonResponse, mark );
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
	}
}