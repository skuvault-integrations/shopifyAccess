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
		public T GetResponse< T >( ShopifyCommand command, string endpoint, Mark mark = null )
		{
			T result;
			var request = this.CreateServiceGetRequest( command, endpoint );
			ShopifyLogger.Log.Trace( "[shopify][{2}] GET Params. ShopName: {0}. Request: {1}", this._commandConfig.ShopName, request.RequestUri, mark.ToStringSafe() );
			using( var response = request.GetResponse() )
				result = this.ParseResponse< T >( response, mark.CreateNewIfBlank() );

			return result;
		}

		public async Task< T > GetResponseAsync< T >( ShopifyCommand command, string endpoint, Mark mark = null )
		{
			T result;
			var request = this.CreateServiceGetRequest( command, endpoint );
			ShopifyLogger.Log.Trace( "[shopify][{2}] GET Params. ShopName: {0}. Request: {1}", this._commandConfig.ShopName, request.RequestUri, mark.ToStringSafe() );
			using( var response = await request.GetResponseAsync() )
				result = this.ParseResponse< T >( response, mark.CreateNewIfBlank() );

			return result;
		}

		public void PutData( ShopifyCommand command, string endpoint, string jsonContent, Mark mark = null )
		{
			Condition.Requires( this._commandConfig, "config" ).IsNotNull();

			var request = this.CreateServicePutRequest( command, endpoint, jsonContent );
			ShopifyLogger.Log.Trace( "[shopify][{2}] PUT Params. ShopName: {0}. Data: {1}", this._commandConfig.ShopName, jsonContent, mark.ToStringSafe() );
			using( var response = ( HttpWebResponse )request.GetResponse() )
				this.LogUpdateInfo( endpoint, response.StatusCode, jsonContent, mark.CreateNewIfBlank() );
		}

		public async Task PutDataAsync( ShopifyCommand command, string endpoint, string jsonContent, Mark mark = null )
		{
			Condition.Requires( this._commandConfig, "config" ).IsNotNull();

			var request = this.CreateServicePutRequest( command, endpoint, jsonContent );
			ShopifyLogger.Log.Trace( "[shopify][{2}] PUT Params. ShopName: {0}. Data: {1}", this._commandConfig.ShopName, jsonContent, mark.ToStringSafe() );
			using( var response = await request.GetResponseAsync() )
				this.LogUpdateInfo( endpoint, ( ( HttpWebResponse )response ).StatusCode, jsonContent, mark.CreateNewIfBlank() );
		}

		public string RequestPermanentToken( string code, Mark mark = null )
		{
			string result;
			var command = ShopifyCommand.GetAccessToken;
			var tokenRequestUrl = new Uri( string.Concat( this._authorizationConfig.Host, command.Command ) );
			var tokenRequestPostContent = string.Format( "client_id={0}&client_secret={1}&code={2}", this._authorizationConfig.ApiKey, this._authorizationConfig.Secret, code );
			var request = this.CreateServicePostRequest( tokenRequestUrl, tokenRequestPostContent );

			using( var response = request.GetResponse() )
				result = this.ParseResponse< TokenRequestResult >( response, mark.CreateNewIfBlank() ).Token;

			return result;
		}

		private T ParseResponse< T >( WebResponse response, Mark mark = null )
		{
			var result = default(T);

			using( var stream = response.GetResponseStream() )
			using( var reader = new StreamReader( stream ) )
			{
				var jsonResponse = reader.ReadToEnd();

				ShopifyLogger.Log.Trace( "[shopify][{2}]\tResponse\t{0} - {1}", response.ResponseUri, jsonResponse, mark.ToStringSafe() );

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
		private void LogUpdateInfo( string endpoint, HttpStatusCode statusCode, string jsonContent, Mark mark = null )
		{
			ShopifyLogger.Log.Trace( "[shopify][{3}]\tPUT/POST call for the endpoint '{0}' has been completed with code '{1}'.\n{2}", endpoint, statusCode, jsonContent, mark.ToStringSafe() );
		}
		#endregion
	}
}