﻿using System;
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

			T result;
			using( var response = request.GetResponse() )
				result = this.ParseResponse< T >( response, mark );

			return result;
		}

		public async Task< T > GetResponseAsync< T >( ShopifyCommand command, string endpoint, Mark mark )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var request = this.CreateServiceGetRequest( command, endpoint );
			this.LogGetRequest( request.RequestUri, mark );

			T result;
			using( var response = await request.GetResponseAsync() )
				result = this.ParseResponse< T >( response, mark );

			return result;
		}

		public void PutData( ShopifyCommand command, string endpoint, string jsonContent, Mark mark )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var request = this.CreateServicePutRequest( command, endpoint, jsonContent );
			this.LogUpdateRequest( request.RequestUri, jsonContent, mark );

			using( var response = ( HttpWebResponse )request.GetResponse() )
				this.LogUpdateResponse( request.RequestUri, response.StatusCode, mark );
		}

		public async Task PutDataAsync( ShopifyCommand command, string endpoint, string jsonContent, Mark mark )
		{
			Condition.Requires( mark, "mark" ).IsNotNull();

			var request = this.CreateServicePutRequest( command, endpoint, jsonContent );
			this.LogUpdateRequest( request.RequestUri, jsonContent, mark );

			using( var response = await request.GetResponseAsync() )
				this.LogUpdateResponse( request.RequestUri, ( ( HttpWebResponse )response ).StatusCode, mark );
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

		private T ParseResponse< T >( WebResponse response, Mark mark )
		{
			var result = default(T);

			using( var stream = response.GetResponseStream() )
			using( var reader = new StreamReader( stream ) )
			{
				var jsonResponse = reader.ReadToEnd();

				var limitMass = response.Headers.GetValues( "HTTP_X_SHOPIFY_SHOP_API_CALL_LIMIT" );
				var limit = limitMass != null && limitMass.Length > 0 ? limitMass[ 0 ] : string.Empty;
				this.LogGetResponse( response.ResponseUri, limit, jsonResponse, mark );

				if( !string.IsNullOrEmpty( jsonResponse ) )
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
			ShopifyLogger.Trace( mark, "GET request\tShopName: {0}\tRequest: {1}", this._commandConfig.ShopName, requestUri );
		}

		private void LogGetResponse( Uri requestUri, string limit, string jsonResponse, Mark mark )
		{
			ShopifyLogger.Trace( mark, "GET response\tShopName: {0}\tRequest: {1}\tLimit: {2}\tResponse: {3}", this._commandConfig.ShopName, requestUri, limit, jsonResponse );
		}

		private void LogUpdateRequest( Uri requestUri, string jsonContent, Mark mark )
		{
			ShopifyLogger.Trace( mark, "PUT request\tShopName: {0}\tRequest: {1}Data: {2}", this._commandConfig.ShopName, requestUri, jsonContent );
		}

		private void LogUpdateResponse( Uri requestUri, HttpStatusCode statusCode, Mark mark )
		{
			ShopifyLogger.Trace( mark, "PUT/POST response\tShopName: {0}\tRequest: {1}\tStatusCode: '{2}'", this._commandConfig.ShopName, requestUri, statusCode );
		}
		#endregion
	}
}