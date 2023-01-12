using System.IO;
using ServiceStack;
using ShopifyAccess.GraphQl.Models;
using ShopifyAccess.GraphQl.Models.ProductVariantsWithInventoryLevelsReport;

namespace ShopifyAccess.GraphQl.Misc
{
	internal static class ProductVariantsWithInventoryLevelsParser
	{
		public static Report Parse( Stream stream )
		{
			var result = new Report();
			ProductVariant lastProductVariant = null;

			foreach( var line in stream.ReadLines() )
			{
				var inventoryLevel = ParseAsInventoryLevel( line );
				if( inventoryLevel.ProductVariantId == null )
				{
					lastProductVariant = ParseAsProductVariant( line );
					result.Add( lastProductVariant );
					continue;
				}

				// By this article: https://shopify.dev/api/usage/bulk-operations/queries#the-jsonl-data-format
				// The order of each connection type is preserved and all nested connections appear after their parents in the file
				// So we can apply new inventory level to the last product variant. Don't need to check __parent id
				lastProductVariant?.InventoryLevels.Add( inventoryLevel );
			}

			return result;
		}

		private static T ParseLine< T >( string content ) where T : class =>
			!string.IsNullOrEmpty( content ) ? content.FromJson< T >() : null;

		private static ProductVariant ParseAsProductVariant( string content ) =>
			ParseLine< ProductVariant >( content );

		private static InventoryLevel ParseAsInventoryLevel( string content ) =>
			ParseLine< InventoryLevel >( content );
	}
}