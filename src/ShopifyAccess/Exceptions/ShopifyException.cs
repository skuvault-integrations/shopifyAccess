using System;
using ShopifyAccess.Models.Configuration.Command;

namespace ShopifyAccess.Exceptions
{
	/// <summary>
	/// Exception thrown if Shopify returns an error.
	/// </summary>
	[ Serializable ]
	public class ShopifyException : Exception
	{
		private readonly ShopifyCommand _shopifyCommand;
		private readonly string _errorMessage;

		public ShopifyException( ShopifyCommand shopifyCommand, string message )
		{
			this._shopifyCommand = shopifyCommand;
			this._errorMessage = message;
		}

		public ShopifyException( string message )
			: this( ShopifyCommand.Unknown, message )
		{
		}

		public ShopifyCommand ShopifyCommand
		{
			get { return this._shopifyCommand; }
		}

		public string ErrorMessage
		{
			get { return this._errorMessage; }
		}

		///<summary>
		///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		///</returns>
		///<filterpriority>2</filterpriority>
		public override string ToString()
		{
			return string.Format( "Failed to execute Shopify command {0}. Error: {1}\n" + base.ToString(), this.ShopifyCommand, this.ErrorMessage );
		}
	}
}