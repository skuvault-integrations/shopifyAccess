using System;

namespace ShopifyAccess.Exceptions
{
	internal class ShopifyException : Exception
	{
		protected ShopifyException( string message, Exception exception = null ) : base( message, exception ) { }
	}

	/// <summary>
	///	Shopify server exception (they can't be reattempted)
	/// </summary>
	internal class ShopifyServerException : ShopifyException
	{
		/// <summary>
		///	Http response status code
		/// </summary>
		private int Code { get; set; }

		internal ShopifyServerException( string message, int code, Exception exception = null ) : base( message, exception )
		{
			this.Code = code;
		}
	}

	/// <summary>
	///	Unauthorized exception
	/// </summary>
	internal class ShopifyUnauthorizedException : ShopifyServerException
	{
		internal ShopifyUnauthorizedException( string message ) : base( message, 401 ) { }
	}
	
	/// <summary>
	///	Unauthorized exception
	/// </summary>
	internal class ShopifyTransientException : ShopifyServerException
	{
		internal ShopifyTransientException( string message, int code ) : base( message, code ) { }
	}
}