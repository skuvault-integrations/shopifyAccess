namespace ShopifyAccess
{
	public interface IShopifyFactory
	{
		IShopifyService CreateService();
	}

	public sealed class ShopifyFactory : IShopifyFactory
	{
		public IShopifyService CreateService()
		{
			return new ShopifyService();
		}
	}
}