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
using ShopifyAccess.Exceptions;
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
		private readonly ShopifyClientCredentials _clientCredentials;
		
		public HttpClient HttpClient { get; private set; }
		public DateTime? LastNetworkActivityTime { get; private set; }
		private const int MaxHttpRequestTimeoutInMinutes = 30;

		#region Constructors
		public WebRequestServices( ShopifyAuthorizationConfig authorizationConfig )
		{
			Condition.Requires( authorizationConfig, "authorizationConfig" ).IsNotNull();

			this._authorizationConfig = authorizationConfig;
			this._clientCredentials = new ShopifyClientCredentials( authorizationConfig.ShopName, "authorization" );
			this.HttpClient = this.CreateHttpClient( this._clientCredentials.AccessToken );
		}

		public WebRequestServices( ShopifyClientCredentials clientCredentials, HttpClient httpClient = null )
		{
			Condition.Requires( clientCredentials, "clientCredentials" ).IsNotNull();

			this._clientCredentials = clientCredentials;
			this.HttpClient = httpClient ?? this.CreateHttpClient( this._clientCredentials.AccessToken );
			var servicePoint = ServicePointManager.FindServicePoint( new Uri( this._clientCredentials.Host ) );
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
			return this.GetResponseAsync<T>(command, endpoint, token, mark, timeout).GetAwaiter().GetResult();
		}

		public ResponsePage< T > GetResponsePage< T >( ShopifyCommand command, string endpoint, CancellationToken token, Mark mark, int timeout )
		{
			return this.GetResponsePageAsync< T >( command, endpoint, token, mark, timeout ).GetAwaiter().GetResult();
		}

		public async Task< T > GetResponseAsync< T >( ShopifyCommand command, string endpoint, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			this.ThrowIfOperationCanceledException( token, mark );

			var uri = this.CreateRequestUri( command, endpoint );
			ShopifyLogger.LogGetRequest( uri, mark, timeout );

			using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) )
			{
				linkedCancellationTokenSource.CancelAfter( timeout );
				this.RefreshLastNetworkActivityTime();
				using( var response = await this.HttpClient.GetAsync( uri, linkedCancellationTokenSource.Token ).ConfigureAwait( false ) )
				{
					await this.ThrowIfErrorAsync( response, mark ).ConfigureAwait( false );
					var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					return ParseResponse< T >( responseContent, response.Headers, uri, mark, timeout );
				}
			}
		}

		public async Task< ResponsePage< T > > GetResponsePageAsync< T >( ShopifyCommand command, string endpoint, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var uri = this.CreateRequestUri( command, endpoint );
			ShopifyLogger.LogGetRequest( uri, mark, timeout );

			this.ThrowIfOperationCanceledException( token, mark );

			using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) )
			{
				linkedCancellationTokenSource.CancelAfter( timeout );
				this.RefreshLastNetworkActivityTime();
				using( var response = await this.HttpClient.GetAsync( uri, linkedCancellationTokenSource.Token ).ConfigureAwait( false ) )
				{
					await this.ThrowIfErrorAsync( response, mark ).ConfigureAwait( false );
					var content = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					return ParsePagedResponse< T >( content, response.Headers, uri, mark, timeout );
				}
			}
		}

		public T PostData< T >( ShopifyCommand command, string jsonContent, CancellationToken token, Mark mark, int timeout )
		{
			return this.PostDataAsync<T>(command, jsonContent, token, mark, timeout).GetAwaiter().GetResult();
		}

		public async Task< T > PostDataAsync< T >( ShopifyCommand command, string jsonContent, CancellationToken token, Mark mark, int timeout )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var url = this.CreateRequestUri( command, endpoint: "" );
			ShopifyLogger.LogUpdateRequest( url, jsonContent, mark, timeout );

			this.ThrowIfOperationCanceledException( token, mark );

			var content = new StringContent( jsonContent, Encoding.UTF8, "application/json" );
			using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( token ) )
			{
				linkedCancellationTokenSource.CancelAfter( timeout );
				this.RefreshLastNetworkActivityTime();
				using( var response = await this.HttpClient.PostAsync( url, content, linkedCancellationTokenSource.Token ).ConfigureAwait( false ) )
				{
					await this.ThrowIfErrorAsync( response, mark ).ConfigureAwait( false );
					var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
					this.RefreshLastNetworkActivityTime();
					return ParseResponse< T >( responseContent, response.Headers, url, mark, timeout );
				}
			}
		}

		public async Task< IEnumerable< T > > GetReportDocumentAsync< T >( string url, Func< Stream, IEnumerable< T > > parseMethod, CancellationToken cancellationToken, Mark mark, int timeout ) where T : class
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			this.ThrowIfOperationCanceledException( cancellationToken, mark );

			var uri = new Uri( url, UriKind.Absolute );
			ShopifyLogger.LogGetRequest( uri, mark, timeout );

			using( var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource( cancellationToken ) )
			{
				linkedCancellationTokenSource.CancelAfter( timeout );
				this.RefreshLastNetworkActivityTime();
				using( var response = await this.HttpClient.GetAsync( uri, HttpCompletionOption.ResponseHeadersRead, linkedCancellationTokenSource.Token ).ConfigureAwait( false ) )
				{
					await this.ThrowIfErrorAsync( response, mark ).ConfigureAwait( false );
					using( var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait( false ) )
					{
						return parseMethod( stream );
					}
				}
			}
		}

		public string RequestPermanentToken( string code, Mark mark )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var command = ShopifyCommand.GetAccessToken;
			var url = new Uri( string.Concat( this._authorizationConfig.Host, command.Command ) );
			var tokenRequestPostContent = string.Format( "client_id={0}&client_secret={1}&code={2}", this._authorizationConfig.ApiKey, this._authorizationConfig.Secret, code );
			var content = new StringContent( tokenRequestPostContent, Encoding.UTF8, "application/x-www-form-urlencoded" );	

			this.RefreshLastNetworkActivityTime();
			var response = this.HttpClient.PostAsync( url, content ).GetAwaiter().GetResult();
			var responseContent = response.Content.ReadAsStringAsync().Result;
			this.RefreshLastNetworkActivityTime();
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
			ShopifyLogger.LogGetResponse< T >( uri, limit, content, mark, timeout );

			return !string.IsNullOrEmpty( content ) ? content.FromJson< T >() : default( T );
		}

		private static ResponsePage< T > ParsePagedResponse< T >( string content, HttpHeaders headers, Uri uri, Mark mark, int timeout )
		{
			var limit = GetLimitFromHeader( headers );
			var nextPageLink = PagedResponseService.GetNextPageQueryStrFromHeader( headers );
			ShopifyLogger.LogGetResponse< T >( uri, limit, nextPageLink, content, mark, timeout );

			var result = !string.IsNullOrEmpty( content ) ? content.FromJson< T >() : default( T );

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

		///  <summary>
		/// 	Handles Shopify server error responses
		///  </summary>
		///  <param name="response">Http response</param>
		///  <param name="mark">Mark</param>
		///  <exception cref="ShopifyTransientException"></exception>
		///  <exception cref="ShopifyUnauthorizedException"></exception>
		///  <exception cref="HttpRequestException"></exception>
		private async Task ThrowIfErrorAsync( HttpResponseMessage response, Mark mark )
		{
			bool IsTransientHttpStatusCode( HttpStatusCode statusCode ) => statusCode >= HttpStatusCode.InternalServerError || statusCode == HttpStatusCode.RequestTimeout;

			var responseStatusCode = response.StatusCode;
			if( response.IsSuccessStatusCode )
				return;

			string message;
			try
			{
				message = await response.Content.ReadAsStringAsync().ConfigureAwait( false );
			}
			catch
			{
				message = "Unable to read response contents";
			}

			ShopifyLogger.LogInvalidStatusCode( ( int )responseStatusCode, message, this._clientCredentials.ShopName, mark );

			if( responseStatusCode == HttpStatusCode.Unauthorized || responseStatusCode == HttpStatusCode.Forbidden )
				throw new ShopifyUnauthorizedException( message, ( int )responseStatusCode );

			if( IsTransientHttpStatusCode( responseStatusCode ) )
				throw new ShopifyTransientException( message, ( int )responseStatusCode );

			response.EnsureSuccessStatusCode(); // will throw HttpRequestException
		}

		private void ThrowIfOperationCanceledException( CancellationToken token, Mark mark )
		{
			if( !token.IsCancellationRequested )
				return;

			var taskCanceledException = new OperationCanceledException( token );
			ShopifyLogger.LogException( taskCanceledException, mark, this._clientCredentials.ShopName );
			token.ThrowIfCancellationRequested();
		}
		#endregion

		#region WebRequest configuration

		private Uri CreateRequestUri( ShopifyCommand command, string endpoint )
		{
			return new Uri( string.Concat( this._clientCredentials.Host, command.Command, endpoint ) );
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