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
			Trace( mark, "GET request\tRequest: {0}", requestUri );
		}

		public static void LogGetResponse( Uri requestUri, string limit, string jsonResponse, Mark mark )
		{
			Trace( mark, "GET response\tRequest: {0}\tLimit: {1}\tResponse: {2}", requestUri, limit, jsonResponse );
		}

		public static void LogGetResponse( Uri requestUri, string limit, string nextPage, string jsonResponse, Mark mark )
		{
			Trace( mark, "GET response\tRequest: {0}\tLimit: {1}\tNext Page: {2}\tResponse: {3}", requestUri, limit, nextPage, jsonResponse );
		}

		public static void LogUpdateRequest( Uri requestUri, string jsonContent, Mark mark, int timeout )
		{
			Trace( mark, "PUT request\tRequest: {0}\tData: {1}", requestUri, jsonContent );
		}

		public static void LogUpdateResponse( Uri requestUri, string limit, HttpStatusCode statusCode, Mark mark )
		{
			Trace( mark, "PUT/POST response\tRequest: {0}\tLimit: {1}\tStatusCode: {2}", requestUri, limit, statusCode );
		}

		public static void LogException( WebException ex, Mark mark, string shopName )
		{
			Trace( ex, mark, "Failed response\tShopName: {0}\tMessage: {1}\tStatus: {2}", shopName, ex.Message, ex.Status );
		}

		public static void LogException( WebException ex, HttpWebResponse response, string jsonResponse, Mark mark )
		{
			Trace( ex, mark, "Failed response\tRequest: {0}\tMessage: {1}\tStatus: {2}\tJsonResponse: {3}",
				response.ResponseUri, ex.Message, response.StatusCode, jsonResponse );
		}
	}
}