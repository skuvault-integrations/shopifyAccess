using System;
using System.Net;
using System.Runtime.CompilerServices;
using Netco.Logging;
using ShopifyAccess.Models;

namespace ShopifyAccess.Misc
{
	public static class ShopifyLogger
	{
		public static ILogger Log{ get; private set; }

		static ShopifyLogger()
		{
			Log = NetcoLogger.GetLogger( "ShopifyLogger" );
		}

		public static void Trace( Mark mark, string format, params object[] args )
		{
			var markStr = string.Format( "[{0}]\t", mark );
			Log.Trace( markStr + format, args );
		}

		public static void Trace( Exception ex, Mark mark, string format, params object[] args )
		{
			var markStr = string.Format( "[{0}]\t", mark );
			Log.Trace( ex, markStr + format, args );
		}

		public static void LogGetRequest( Uri requestUri, Mark mark, int timeout )
		{
			Trace( mark, "GET request\tRequest: {0} with timeout {1}ms", requestUri, timeout );
		}

		/// <summary>
		/// Log the response of a GET call
		/// </summary>
		/// <param name="requestUri"></param>
		/// <param name="limit"></param>
		/// <param name="jsonResponse"></param>
		/// <param name="mark"></param>
		/// <param name="timeout"></param>
		/// <typeparam name="TResponseType">The type of object returned in response from Shopify. Needed to transform the response for logging</typeparam>
		public static void LogGetResponse< TResponseType >( Uri requestUri, string limit, string jsonResponse, Mark mark, int timeout )
		{
			var contentForLogs = jsonResponse.ToLogContents< TResponseType >();
			Trace( mark, "GET response\tRequest: {0} with timeout {1}ms\tLimit: {2}\tResponse: {3}", 
				requestUri, timeout, limit, contentForLogs );
		}

		/// <summary>
		/// Log the response of a GET call for a specific page
		/// </summary>
		/// <param name="requestUri"></param>
		/// <param name="limit"></param>
		/// <param name="nextPage"></param>
		/// <param name="jsonResponse"></param>
		/// <param name="mark"></param>
		/// <param name="timeout"></param>
		/// <typeparam name="TResponseType">The type of object returned in response from Shopify. Needed to transform the response for logging</typeparam>
		public static void LogGetResponse< TResponseType >( Uri requestUri, string limit, string nextPage, string jsonResponse, Mark mark, int timeout )
		{
			var contentForLogs = jsonResponse.ToLogContents< TResponseType >();
			Trace( mark, "GET response\tRequest: {0} with timeout {1}ms\tLimit: {2}\tNext Page: {3}\tResponse: {4}", 
				requestUri, timeout, limit, nextPage, contentForLogs );
		}

		public static void LogUpdateRequest( Uri requestUri, string jsonContent, Mark mark, int timeout )
		{
			Trace( mark, "PUT request\tRequest: {0} with timeout {1}ms\tData: {2}", requestUri, timeout, jsonContent );
		}

		public static void LogUpdateResponse( Uri requestUri, string limit, HttpStatusCode statusCode, Mark mark, int timeout )
		{
			Trace( mark, "PUT/POST response\tRequest: {0} with timeout {1}ms\tLimit: {2}\tStatusCode: {3}", requestUri, timeout, limit, statusCode );
		}

		public static void LogException( Exception ex, Mark mark, string shopName )
		{
			Trace( ex, mark, "Failed response\tShopName: {0}\tMessage: {1}", shopName, ex.Message );
		}

		public static void LogInvalidStatusCode( int statusCode, string message, string shopName, Mark mark )
		{
			Trace( mark, "Failed response\tShopName: {0}\tMessage: {1}\tStatus: {2}", shopName, message, statusCode );
		}

		public static void LogOperationStart( string shopName, Mark mark, string message = null, [ CallerMemberName ] string callerMethodName = null )
		{
			Trace( mark, "Shop: {0}. Start {1}. {2}", shopName, callerMethodName, message ?? string.Empty );
		}

		public static void LogOperationEnd( string shopName, Mark mark, [ CallerMemberName ] string callerMethodName = null )
		{
			Trace( mark, "Shop: {0}. End {1}", shopName, callerMethodName );
		}
		
		public static void LogOperation( string shopName, Mark mark, string message, [ CallerMemberName ] string callerMethodName = null )
		{
			Trace( mark, "Shop: {0}. {1}: {2}", shopName, callerMethodName, message );
		}
	}
}