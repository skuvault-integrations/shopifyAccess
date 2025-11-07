using NUnit.Framework.Internal;

namespace ShopifyAccessTests.GraphQl.Helpers
{
	internal static class GraphQlIdsGenerator
	{
		private static readonly Randomizer _randomizer = new Randomizer();

		internal static string CreateProductId( long? productId = null )
		{
			return $"gid://shopify/Product/{productId ?? _randomizer.NextLong()}";
		}
	}
}