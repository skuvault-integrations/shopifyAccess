using System;
using ServiceStack;
using ShopifyAccess.Models.Product;

namespace ShopifyAccess.Helpers
{
	//TODO GUARD-2710 Add tests: one product (string template), just BodyHtml, ensure product isn't modified
	//	Could do the entire row of 250 but it's huge to check in
	public static class LogHelper
	{
		private const int MaxFieldValueLength = 300;

		/// <summary>
		/// TODO: RegEx implementation
		/// </summary>
		/// <param name="contentsJson"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public static string ToLogContents( this string contentsJson )
		{
			throw new NotImplementedException();
		}

		//TODO GUARD-2710 Slav says serialization/deserialization is expensive. Better use RegEx
		//	Create an alternative implementation using RegEx (above) and compare performance on a batch of 250 products

		/// <summary>
		/// Change field values for logs. For example, to truncate long ones or obfuscate sensitive information.
		/// </summary>
		/// <param name="contents"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static string ToLogContents< T >( T contents ) where T: class
		{
			TruncateProductLongFieldValues( contents );
			return contents.ToJson();
		}

		/// <summary>
		/// Truncate field values that tend to be long and don't have to be logged in their entirety
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
					product.BodyHtml = product.BodyHtml.Substring( MaxFieldValueLength );
				}
			}
			//TODO GUARD-2710 Rider says this value isn't used but it should be available outside since it's passed by ref.
			//Confirm with tests that it's modified locally but no in the caller
			contents = products as T;
		}
	}
}