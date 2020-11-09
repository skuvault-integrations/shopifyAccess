using System;

namespace ShopifyAccess.Misc
{
	public static class Extensions
	{
		public static string GetUrlWithoutQueryPart( this string rawUrl )
		{
			if ( string.IsNullOrWhiteSpace( rawUrl ) )
				return rawUrl;

			try
			{
				var url = new Uri( rawUrl );
				return url.GetLeftPart( UriPartial.Path );
			}
			catch { }

			return rawUrl;
		}
	}
}