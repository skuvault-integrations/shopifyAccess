using System;

namespace ShopifyAccess.Exceptions
{
	internal class ShopifyException: Exception
	{
		protected ShopifyException( string message, Exception exception = null ): base( message, exception )
		{
		}
	}

	/// <summary>
	///	Shopify server exception
	/// </summary>
	internal class ShopifyServerException: ShopifyException
	{
		/// <summary>
		///	Http response status code
		/// </summary>
		private int Code{ get; set; }

		internal ShopifyServerException( string message, int code, Exception exception = null ): base( message, exception )
		{
			this.Code = code;
		}
	}

	/// <summary>
	///	Unauthorized exception (codes 401 or 403). Can't be reattempted
	/// </summary>
	internal class ShopifyUnauthorizedException: ShopifyServerException
	{
		internal ShopifyUnauthorizedException( string message, int code ): base( message, code )
		{
		}
	}

	/// <summary>
	///	Transient exception (Codes 408 or 5xx). These exceptions can be reattempted
	/// </summary>
	internal class ShopifyTransientException: ShopifyServerException
	{
		internal ShopifyTransientException( string message, int code ): base( message, code )
		{
		}
	}
}