using ShopifyAccess.Models;
using ShopifyAccess.Models.Configuration.Command;

namespace ShopifyAccess
{
	public interface IShopifyFactory
	{
		IShopifyService CreateService( ShopifyClientCredentials clientCredentials, ShopifyTimeouts operationsTimeouts = null );
	}

	public sealed class ShopifyFactory : IShopifyFactory
	{
		private readonly ShopifyCommandFactory _commandFactory;

		public ShopifyFactory( ShopifyApiVersion apiVersion )
		{
			this._commandFactory = new ShopifyCommandFactory( apiVersion );
		}
	
		public IShopifyService CreateService( ShopifyClientCredentials clientCredentials, ShopifyTimeouts operationsTimeouts = null )
		{
			return new ShopifyService( clientCredentials, operationsTimeouts ?? new ShopifyTimeouts(), this._commandFactory );
		}
	}
}