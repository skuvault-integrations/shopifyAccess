using System;

namespace ShopifyAccess.GraphQl.Misc
{
	internal static class GraphQlObjectIdsParser
	{
		private const string GraphQlInventoryItemGidPrefix = "gid://shopify/InventoryItem/";
		private const string GraphQlLocationGidPrefix = "gid://shopify/Location/";

		internal static long GetId(this GraphQlId graphQlId, string gid )
		{
			if( string.IsNullOrWhiteSpace( gid ) || !gid.StartsWith( graphQlId.GidPrefix ) )
			{
				throw new ArgumentException( $"Wrong {graphQlId.Name} gid provided", nameof(gid) );
			}

			var id = gid.Remove( 0, graphQlId.GidPrefix.Length );
			return long.Parse( id );
		}

		//TODO GUARD-2649 Refactor these like ProductVariant was refactored
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

	internal class GraphQlId
	{
		internal static GraphQlId ProductVariant = new GraphQlId("product variant", "gid://shopify/ProductVariant/");
		internal readonly string Name;
		internal readonly string GidPrefix;

		public GraphQlId(string name, string gidPrefix)
		{
			this.Name = name;
			this.GidPrefix = gidPrefix;
		}
	}
}