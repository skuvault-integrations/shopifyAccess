using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace ShopifyAccess.Services
{
	public static class PagedResponseService
	{
		public static string GetNextPageQueryStrFromHeader( HttpHeaders responseHeaders )
		{
			IEnumerable< string > linkHeaders;
			responseHeaders.TryGetValues( "Link", out linkHeaders );
			if( linkHeaders == null || !linkHeaders.Any() )
				return string.Empty;

			var links = linkHeaders.First().Split(',').Select( x => new PageLink ( x.Split(';') ) ).ToArray();
			if( !links.Any() || links.Last().Relationship != "rel=\"next\"" )
				return string.Empty;

			var urlWithAngleBraces = links.Last().Url;
			var url = urlWithAngleBraces.Substring( 1, urlWithAngleBraces.Length - 2 );
			return new Uri( url ).Query;
		}
	}

	internal class PageLink
	{
		public PageLink( string[] linkParts )
		{
			this.Url = linkParts[ 0 ].Trim();
			this.Relationship = linkParts[ 1 ].Trim();
		}

		public string Url { get; private set; }
		public string Relationship { get; private set; }
	}

	public class ResponsePage< T >
	{
		public T Response;
		public string NextPageQueryStr;
	}
}
