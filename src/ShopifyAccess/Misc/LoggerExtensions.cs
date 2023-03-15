using ShopifyAccess.Models.Product;
using System.Text.RegularExpressions;

namespace ShopifyAccess.Misc
{
	public static class LoggerExtensions
	{
		internal const int MaxFieldValueLength = 300;
		internal const string TruncationIndicator = "...";
		
		/// <summary>
		/// RegEx to match "body_html":"{at most MaxFieldValueLength length or within the nearest word boundary (to avoid breaking JSON)}{capture the "tail" of long body_html}","vendor".<br />
		/// (?&lt;=foo)	Lookbehind	Asserts but not captures what immediately precedes the current position<br />
		/// (?=foo)	Lookahead	Asserts but not captures what immediately follows the current position<br />
		/// (...?)	Greedy		Get as much as possible
		/// </summary>
		private static readonly Regex _regExBodyHtmlTail = new Regex( @"(?<=""body_html"":"".{" + ( MaxFieldValueLength - 20 ) + "," + MaxFieldValueLength + @"}\w)(.*?)(?="",""vendor"")", RegexOptions.Compiled );

		/// <summary>
		/// Prepare field values for logs. For example, to truncate long ones or obfuscate sensitive information.
		/// </summary>
		/// <param name="contentsJson"></param>
		/// <returns></returns>
		/// <typeparam name="TResponseType">The type of object returned in response from Shopify. Needed to only transform response of correct type (not all).</typeparam>
		public static string ToLogContents< TResponseType >( this string contentsJson )
		{
			if( typeof( TResponseType ) == typeof( ShopifyProducts ) )
			{
				return TruncateProductLongFieldValues( contentsJson );
			}

			return contentsJson;
		}

		private static string TruncateProductLongFieldValues( string contentsJson )
		{
			return _regExBodyHtmlTail.Replace( contentsJson, TruncationIndicator );
		}
	}
}