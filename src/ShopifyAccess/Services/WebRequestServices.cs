using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
		public DateTime? LastNetworkActivityTime { get; private set; }
		private const int MaxHttpRequestTimeoutInMinutes = 30;

		#region Constructors
		public WebRequestServices( ShopifyAuthorizationConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._authorizationConfig = config;
			this._commandConfig = new ShopifyCommandConfig( config.ShopName, "authorization" );
			this.HttpClient = this.CreateHttpClient( this._commandConfig.AccessToken );
		}

		public WebRequestServices( ShopifyCommandConfig config )
		{
			Condition.Requires( config, "config" ).IsNotNull();

			this._commandConfig = config;
			this.HttpClient = this.CreateHttpClient( this._commandConfig.AccessToken );
			var servicePoint = ServicePointManager.FindServicePoint( new Uri( this._commandConfig.Host ) );
			servicePoint.ConnectionLimit = 1000;
		}

		private HttpClient CreateHttpClient( string accessToken )
		{
			var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes( MaxHttpRequestTimeoutInMinutes ) };
			if ( !string.IsNullOrWhiteSpace( accessToken ) ) 
			{
				httpClient.DefaultRequestHeaders.Remove( "X-Shopify-Access-Token" );
				httpClient.DefaultRequestHeaders.Add( "X-Shopify-Access-Token", accessToken );
			}
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
				this.LogAndThrowTaskCanceledException( mark );
			}

			return this.ParseException( mark, timeout, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					RefreshLastNetworkActivityTime();
					var response = await this.HttpClient.GetAsync( uri, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					var content = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					RefreshLastNetworkActivityTime();
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
				this.LogAndThrowTaskCanceledException( mark );
			}

			return this.ParseException( mark, timeout, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					RefreshLastNetworkActivityTime();
					var response = await this.HttpClient.GetAsync( uri, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					var content = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					RefreshLastNetworkActivityTime();
					return ParsePagedResponse< T >( content, response.Headers, uri, mark, timeout );
				}
			} ).Result;
		}

		public async Task< T > GetResponseAsync< T >( ShopifyCommand command, string endpoint, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			if( token.IsCancellationRequested )
			{
				this.LogAndThrowTaskCanceledException( mark );
			}

			var uri = this.CreateRequestUri( command, endpoint );
			ShopifyLogger.LogGetRequest( uri, mark, timeout );

			return await this.ParseExceptionAsync( mark, timeout, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					RefreshLastNetworkActivityTime();
					var response = await this.HttpClient.GetAsync( uri, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					RefreshLastNetworkActivityTime();
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
				this.LogAndThrowTaskCanceledException( mark );
			}

			return await this.ParseExceptionAsync( mark, timeout, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					RefreshLastNetworkActivityTime();
					var response = await this.HttpClient.GetAsync( uri, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					var content = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					RefreshLastNetworkActivityTime();
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
				this.LogAndThrowTaskCanceledException( mark );
			}

			this.ParseException( mark, timeout, () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					var content = new StringContent( jsonContent, Encoding.UTF8, "application/json" );
					RefreshLastNetworkActivityTime();
					var response = this.HttpClient.PutAsync( uri, content, linkedCancellationTokenSource.Token ).GetAwaiter().GetResult();
					RefreshLastNetworkActivityTime();
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
				this.LogAndThrowTaskCanceledException( mark );
			}

			await this.ParseExceptionAsync( mark, timeout, async () =>
			{
				using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
				{ 
					linkedCancellationTokenSource.CancelAfter( timeout );
					var content = new StringContent( jsonContent, Encoding.UTF8, "application/json" );
					RefreshLastNetworkActivityTime();
					var response = await this.HttpClient.PutAsync( uri, content, linkedCancellationTokenSource.Token ).ConfigureAwait( false );
					RefreshLastNetworkActivityTime();
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
				this.LogAndThrowTaskCanceledException( mark );
			}


			var content = new StringContent( jsonContent, Encoding.UTF8, "application/json" );	
			HttpResponseMessage response;
			using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
			{ 
				linkedCancellationTokenSource.CancelAfter( timeout );
				RefreshLastNetworkActivityTime();
				response = this.HttpClient.PostAsync( url, content, linkedCancellationTokenSource.Token ).GetAwaiter().GetResult();
			}
			var responseContent = response.Content.ReadAsStringAsync().Result;
			RefreshLastNetworkActivityTime();
			ParseResponse< T >( responseContent, response.Headers, url, mark, timeout );
		}

		public async Task< T > PostDataAsync< T >( ShopifyCommand command, string jsonContent, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var url = this.CreateRequestUri( command, endpoint: "" );
			ShopifyLogger.LogUpdateRequest( url, jsonContent, mark, timeout );

			if( token.IsCancellationRequested )
			{
				this.LogAndThrowTaskCanceledException( mark );
			}


			var content = new StringContent( jsonContent, Encoding.UTF8, "application/json" );	
			HttpResponseMessage response;
			using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) ) 
			{ 
				linkedCancellationTokenSource.CancelAfter( timeout );
				RefreshLastNetworkActivityTime();
				response = await this.HttpClient.PostAsync( url, content, linkedCancellationTokenSource.Token );
			}
			var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
			RefreshLastNetworkActivityTime();
			var result = ParseResponse< T >( responseContent, response.Headers, url, mark, timeout );
			return result;
		}

		public string RequestPermanentToken( string code, Mark mark )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var command = ShopifyCommand.GetAccessToken;
			var url = new Uri( string.Concat( this._authorizationConfig.Host, command.Command ) );
			var tokenRequestPostContent = string.Format( "client_id={0}&client_secret={1}&code={2}", this._authorizationConfig.ApiKey, this._authorizationConfig.Secret, code );
			var content = new StringContent( tokenRequestPostContent, Encoding.UTF8, "application/x-www-form-urlencoded" );	

			RefreshLastNetworkActivityTime();
			var response = this.HttpClient.PostAsync( url, content ).GetAwaiter().GetResult();
			var responseContent = response.Content.ReadAsStringAsync().Result;
			RefreshLastNetworkActivityTime();
			var result = ParseResponse< TokenRequestResult >( responseContent, response.Headers, url, mark ).Token;

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
				throw this.LogException( ex, mark );
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
				throw this.LogException( ex, mark );
			}
			catch( TaskCanceledException )
			{
				ShopifyLogger.LogTimeoutException( mark, this._commandConfig.ShopName, timeout );
				throw;
			}
		}

		private WebException LogException( WebException ex, Mark mark )
		{
			if( ex.Response == null || ex.Status != WebExceptionStatus.ProtocolError ||
			    ex.Response.ContentType == null || ex.Response.ContentType.Contains( "text/html" ) )
			{
				ShopifyLogger.LogWebException( ex, mark, this._commandConfig.ShopName );
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

		private void LogAndThrowTaskCanceledException( Mark mark )
		{
			var taskCanceledException = new TaskCanceledException();
			ShopifyLogger.LogException( taskCanceledException, mark, this._commandConfig.ShopName );
			throw taskCanceledException;
		}
		#endregion

		#region WebRequest configuration

		private Uri CreateRequestUri( ShopifyCommand command, string endpoint )
		{
			return new Uri( string.Concat( this._commandConfig.Host, command.Command, endpoint ) );
		}
		#endregion

		/// <summary>
		///	Call before every API request to server or after handling the response, to save last network activity time
		/// </summary>
		private void RefreshLastNetworkActivityTime()
		{
			this.LastNetworkActivityTime = DateTime.UtcNow;
		}
	}
}