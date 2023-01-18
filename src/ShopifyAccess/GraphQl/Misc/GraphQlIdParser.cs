using System;

namespace ShopifyAccess.GraphQl.Misc
{
	internal sealed class GraphQlIdParser
	{
		private readonly string Name;
		private readonly string GidPrefix;

		internal static readonly GraphQlIdParser ProductVariant = new GraphQlIdParser( "Product Variant", "gid://shopify/ProductVariant/" );
		internal static readonly GraphQlIdParser InventoryItem = new GraphQlIdParser( "Inventory Item", "gid://shopify/InventoryItem/" );
		internal static readonly GraphQlIdParser Location = new GraphQlIdParser( "Location", "gid://shopify/Location/" );

		private GraphQlIdParser( string name, string gidPrefix )
		{
			this.Name = name;
			this.GidPrefix = gidPrefix;
		}

		internal long GetId( string gid )
		{
			if( string.IsNullOrWhiteSpace( gid ) || !gid.StartsWith( this.GidPrefix ) )
			{
				throw new ArgumentException( $"Wrong {this.Name} gid provided", nameof(gid) );
			}

			var id = gid.Remove( 0, this.GidPrefix.Length );
			return long.Parse( id );
		}
	}
}