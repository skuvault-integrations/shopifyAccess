using System;

namespace ShopifyAccess.Misc
{
	public static class Extensions
	{
		public static string GetUrlWithoutQueryPart( this string rawUrl )
		{
			if ( string.IsNullOrWhiteSpace( rawUrl ) )
				return rawUrl;

			Uri url;
			if ( Uri.TryCreate( rawUrl, UriKind.Absolute, out url ) )
			{
				return url.GetLeftPart( UriPartial.Path );
			}

			return rawUrl;
		}
	}
}