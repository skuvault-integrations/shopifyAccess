using System;
using System.Net;
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

		public static void LogGetResponse( Uri requestUri, string limit, string jsonResponse, Mark mark, int timeout )
		{
			Trace( mark, "GET response\tRequest: {0} with timeout {1}ms\tLimit: {2}\tResponse: {3}", requestUri, timeout, limit, jsonResponse );
		}

		public static void LogGetResponse( Uri requestUri, string limit, string nextPage, string jsonResponse, Mark mark, int timeout )
		{
			Trace( mark, "GET response\tRequest: {0} with timeout {1}ms\tLimit: {2}\tNext Page: {3}\tResponse: {4}", requestUri, timeout, limit, nextPage, jsonResponse );
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

		public static void LogWebException( WebException ex, Mark mark, string shopName )
		{
			Trace( ex, mark, "Failed response\tShopName: {0}\tMessage: {1}\tStatus: {2}", shopName, ex.Message, ex.Status );
		}

		public static void LogTimeoutException( Mark mark, string shopName, int timeout )
		{
			Trace( mark, "Request timed out\tTimeout: {0}ms\tShopName: {1}", timeout, shopName );
		}

		public static void LogException( WebException ex, HttpWebResponse response, string jsonResponse, Mark mark )
		{
			Trace( ex, mark, "Failed response\tRequest: {0}\tMessage: {1}\tStatus: {2}\tJsonResponse: {3}",
				response.ResponseUri, ex.Message, response.StatusCode, jsonResponse );
		}
	}
}