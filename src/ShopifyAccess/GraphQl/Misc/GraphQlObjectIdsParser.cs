using System;

namespace ShopifyAccess.GraphQl.Misc
{
	internal static class GraphQlObjectIdsParser
	{
		private const string GraphQlProductVariantGidPrefix = "gid://shopify/ProductVariant/";
		private const string GraphQlInventoryItemGidPrefix = "gid://shopify/InventoryItem/";
		private const string GraphQlLocationGidPrefix = "gid://shopify/Location/";

		public static long GetProductVariantId( string gid )
		{
			if( string.IsNullOrWhiteSpace( gid ) || !gid.StartsWith( GraphQlProductVariantGidPrefix ) )
			{
				throw new ArgumentException( "Wrong product variant gid provided", nameof(gid) );
			}

			var id = gid.Remove( 0, GraphQlProductVariantGidPrefix.Length );
			return long.Parse( id );
		}

		public static long GetInventoryItemId( string gid )
		{
			if( string.IsNullOrWhiteSpace( gid ) || !gid.StartsWith( GraphQlInventoryItemGidPrefix ) )
			{
				throw new ArgumentException( "Wrong inventory item gid provided", nameof(gid) );
			}

			var id = gid.Remove( 0, GraphQlInventoryItemGidPrefix.Length );
			return long.Parse( id );
		}

		public static long GetLocationId( string gid )
		{
			if( string.IsNullOrWhiteSpace( gid ) || !gid.StartsWith( GraphQlLocationGidPrefix ) )
			{
				throw new ArgumentException( "Wrong location gid provided", nameof(gid) );
			}

			var id = gid.Remove( 0, GraphQlLocationGidPrefix.Length );
			return long.Parse( id );
		}
	}
}