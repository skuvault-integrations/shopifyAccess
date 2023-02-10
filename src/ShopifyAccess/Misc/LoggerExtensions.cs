using ServiceStack;
using ShopifyAccess.Models.Product;
using System.Text.RegularExpressions;

namespace ShopifyAccess.Misc
{
	public static class LoggerExtensions
	{
		internal const int MaxFieldValueLength = 300;
		internal const string TruncationIndicator = "...";

		/// <summary>
		/// Prepare field values for logs. For example, to truncate long ones or obfuscate sensitive information.
		/// </summary>
		/// <param name="contentsJson"></param>
		/// <returns></returns>
		public static string ToLogContents< T >( this string contentsJson )
		{
			return TruncateProductLongFieldValues< T >( contentsJson );
		}

		private static string TruncateProductLongFieldValues< T >( string contentsJson )
		{
			if( typeof( T ) != typeof( ShopifyProducts ) )
				return contentsJson;

			//RegEx to match "body_html":"{at most MaxFieldValueLength length or within the nearest word boundary (to avoid breaking JSON)}{capture the "tail" of long body_html}","vendor"
			//(?<=foo)	Lookbehind	Asserts but not captures what immediately precedes the current position
			//(?=foo)	Lookahead	Asserts but not captures what immediately follows the current position
			//(...?)	Greedy		Get as much as possible
			var regExBodyHtmlTail = new Regex( @"(?<=""body_html"":"".{" + ( MaxFieldValueLength - 20 ) + "," + MaxFieldValueLength + @"}\w)(.*?)(?="",""vendor"")", RegexOptions.Compiled );
			return regExBodyHtmlTail.Replace( contentsJson, TruncationIndicator );
		}
	}
}