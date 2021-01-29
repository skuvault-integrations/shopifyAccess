using ShopifyAccess.Models;
using ShopifyAccess.Models.Configuration.Command;

namespace ShopifyAccess
{
	public interface IShopifyFactory
	{
		IShopifyService CreateService( ShopifyCommandConfig config, ShopifyTimeouts operationsTimeouts = null );
	}

	public sealed class ShopifyFactory : IShopifyFactory
	{
		public IShopifyService CreateService( ShopifyCommandConfig config, ShopifyTimeouts operationsTimeouts = null )
		{
			return new ShopifyService( config, operationsTimeouts ?? new ShopifyTimeouts() );
		}
	}
}