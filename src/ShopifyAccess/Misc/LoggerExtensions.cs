using ServiceStack;
using ShopifyAccess.Models.Product;
using System.Text.RegularExpressions;

namespace ShopifyAccess.Misc
{
	//TODO GUARD-2710 Temporarily have both RegEx & strongly-typed truncation implementations. Delete string ToLogContents() or string ToLogContents< T >()
	public static class LoggerExtensions
	{
		internal const int MaxFieldValueLength = 300;

		/// <summary>
		/// Prepare field values for logs. For example, to truncate long ones or obfuscate sensitive information.
		/// Raw Json implementation, using RegEx
		/// </summary>
		/// <param name="contentsJson"></param>
		/// <returns></returns>
		public static string ToLogContents( this string contentsJson )
		{
			return TruncateProductLongFieldValues( contentsJson );
		}

		private static string TruncateProductLongFieldValues( string contentsJson )
		{
			//RegEx to match "body_html":"{at most MaxFieldValueLength length or within the nearest word boundary (to avoid breaking JSON)}{capture the "tail" of long body_html}","vendor"
			//(?<=foo)	Lookbehind	Asserts but not captures what immediately precedes the current position
			//(?=foo)	Lookahead	Asserts but not captures what immediately follows the current position
			//(...?)	Greedy		Get as much as possible
			var regExBodyHtmlTail = new Regex( @"(?<=""body_html"":"".{" + ( MaxFieldValueLength - 20 ) + "," + MaxFieldValueLength + @"}\w)(.*?)(?="",""vendor"")", RegexOptions.Compiled );
			return regExBodyHtmlTail.Replace( contentsJson, "" );
		}

		/// <summary>
		/// Prepare field values for logs. For example, to truncate long ones or obfuscate sensitive information.
		/// Strongly-typed implementation that requires extra serialization
		/// </summary>
		/// <param name="contents"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static string ToLogContents< T >( this T contents ) where T: class
		{
			TruncateProductLongFieldValues( contents );
			return contents.ToJson();
		}

		/// <summary>
		/// Truncate field values that tend to be long and don't have to be logged in their entirety.
		/// </summary>
		/// <param name="contents"></param>
		/// <typeparam name="T"></typeparam>
		private static void TruncateProductLongFieldValues< T >( T contents ) where T: class
		{
			var products = contents as ShopifyProducts;
			if( products != null )
			{
				foreach( var product in products.Products )
				{
					product.BodyHtml = product.BodyHtml.Substring( 0, MaxFieldValueLength );
				}
			}
			contents = products as T;
		}
	}
}