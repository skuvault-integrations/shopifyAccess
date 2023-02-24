using System;

namespace ShopifyAccess.Exceptions
{
	/// <summary>
	///	Shopify Http Request Exception
	/// </summary>
	internal class ShopifyHttpRequestException: Exception
	{
		/// <summary>
		///	Http response status code
		/// </summary>
		private int Code{ get; set; }

		internal ShopifyHttpRequestException( string message, int code, Exception exception = null ): base( message, exception )
		{
			this.Code = code;
		}
	}

	/// <summary>
	///	Unauthorized exception (codes 401 or 403). Can't be reattempted
	/// </summary>
	internal class ShopifyUnauthorizedException: ShopifyHttpRequestException
	{
		internal ShopifyUnauthorizedException( string message, int code ): base( message, code )
		{
		}
	}

	/// <summary>
	///	Transient exception (Codes 408 or 5xx). These exceptions can be reattempted
	/// </summary>
	internal class ShopifyTransientException: ShopifyHttpRequestException
	{
		internal ShopifyTransientException( string message, int code ): base( message, code )
		{
		}
	}
}