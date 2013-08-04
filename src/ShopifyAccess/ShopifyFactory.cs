using ShopifyAccess.Models.Configuration.Command;

namespace ShopifyAccess
{
	public interface IShopifyFactory
	{
		IShopifyService CreateService(ShopifyCommandConfig config);
	}

	public sealed class ShopifyFactory : IShopifyFactory
	{
		public IShopifyService CreateService( ShopifyCommandConfig config )
		{
			return new ShopifyService( config );
		}
	}
}