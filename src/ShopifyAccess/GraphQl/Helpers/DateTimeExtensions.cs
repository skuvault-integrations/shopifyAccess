using System;

namespace ShopifyAccess.GraphQl.Helpers
{
	internal static class DateTimeExtensions
	{
		public static string ToIso8601( this DateTime dateTime )
		{
			return dateTime.ToString( "o" );
		}
	}
}