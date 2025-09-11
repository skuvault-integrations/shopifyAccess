using System;

namespace ShopifyAccess.GraphQl.Helpers
{
	internal sealed class GraphQlIdParser
	{
		private readonly string Name;
		private readonly string GidPrefix;

		internal static readonly GraphQlIdParser ProductVariant = new GraphQlIdParser( "Product Variant", "gid://shopify/ProductVariant/" );
		internal static readonly GraphQlIdParser InventoryItem = new GraphQlIdParser( "Inventory Item", "gid://shopify/InventoryItem/" );
		internal static readonly GraphQlIdParser Location = new GraphQlIdParser( "Location", "gid://shopify/Location/" );
		internal static readonly GraphQlIdParser Order = new GraphQlIdParser( "Order", "gid://shopify/Order/" );
		internal static readonly GraphQlIdParser LineItem = new GraphQlIdParser( "LineItem", "gid://shopify/LineItem/" );
		internal static readonly GraphQlIdParser Refund = new GraphQlIdParser( "Refund", "gid://shopify/Refund/" );
		internal static readonly GraphQlIdParser Fulfillment = new GraphQlIdParser( "Fulfillment", "gid://shopify/Fulfillment/" );

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