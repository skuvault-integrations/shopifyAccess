using System;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.GraphQl.Misc;

namespace ShopifyAccessTests.GraphQl.Misc
{
	[ TestFixture ]
	public class GraphQlIdParserTests
	{
		[ Test ]
		public void GetId_TrowsArgumentException_WhenWrongGidProvided()
		{
			// Arrange
			const string gid = "wrong_gid";

			// Act, Assert
			GraphQlIdParser.Location.Invoking( p => p.GetId( gid ) ).Should().Throw< ArgumentException >();
		}

		[ Test ]
		public void GetId_ReturnsLocationId_WhenCorrectLocationGidProvided()
		{
			// Arrange
			const int locationId = 777;
			var gid = "gid://shopify/Location/" + locationId;

			// Act
			var result = GraphQlIdParser.Location.GetId( gid );

			// Assert
			result.Should().Be( locationId );
		}

		[ Test ]
		public void GetId_ReturnsProductVariantId_WhenCorrectProductVariantGidProvided()
		{
			// Arrange
			const int productVariantId = 777;
			var gid = "gid://shopify/ProductVariant/" + productVariantId;

			// Act
			var result = GraphQlIdParser.ProductVariant.GetId( gid );

			// Assert
			result.Should().Be( productVariantId );
		}

		[ Test ]
		public void GetId_ReturnsInventoryItemId_WhenCorrectInventoryItemGidProvided()
		{
			// Arrange
			const int inventoryItemId = 777;
			var gid = "gid://shopify/InventoryItem/" + inventoryItemId;

			// Act
			var result = GraphQlIdParser.InventoryItem.GetId( gid );

			// Assert
			result.Should().Be( inventoryItemId );
		}
	}
}