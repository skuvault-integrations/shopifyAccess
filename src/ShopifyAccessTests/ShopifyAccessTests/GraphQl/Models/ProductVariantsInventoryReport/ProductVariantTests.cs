using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.GraphQl.Models.ProductVariantsInventory;

namespace ShopifyAccessTests.GraphQl.Models.ProductVariantsInventoryReport
{
	[ TestFixture ]
	public class ProductVariantTests
	{
		private const int InventoryItemId = 777;
		private const int ProductVariantId = 888;
		private const string Sku = "testSku";

		[ Test ]
		public void ToShopifyProductVariant_ReturnsShopifyProductVariant()
		{
			// Arrange
			var productVariant = GetProductVariant();

			// Act
			var result = productVariant.ToShopifyProductVariant();

			// Assert
			result.Id.Should().Be( ProductVariantId );
			result.InventoryItemId.Should().Be( InventoryItemId );
			result.Sku.Should().Be( Sku );
		}

		[ Test ]
		public void ToShopifyProductVariant_ReturnsShopifyProductVariantWithInventoryLevel()
		{
			// Arrange
			var available = 111;
			var locationId = 222;
			var inventoryLevel = GetInventoryLevel( available, locationId );
			var productVariant = GetProductVariant();
			productVariant.InventoryItem.InventoryLevelsNodes.Nodes.Add( inventoryLevel );

			// Act
			var result = productVariant.ToShopifyProductVariant();

			// Assert
			result.InventoryLevels.InventoryLevels.Should().HaveCount( 1 );
			result.InventoryLevels.InventoryLevels[ 0 ].Available.Should().Be( available );
			result.InventoryLevels.InventoryLevels[ 0 ].LocationId.Should().Be( locationId );
			result.InventoryLevels.InventoryLevels[ 0 ].InventoryItemId.Should().Be( InventoryItemId );
		}

		[ Test ]
		public void ToShopifyProductVariant_ReturnsShopifyProductVariantWithInventoryLevels()
		{
			// Arrange
			var inventoryLevel1 = GetInventoryLevel( 1, 2 );
			var inventoryLevel2 = GetInventoryLevel( 2, 2 );
			var inventoryLevel3 = GetInventoryLevel( 3, 3 );
			var productVariant = GetProductVariant();
			productVariant.InventoryItem.InventoryLevelsNodes.Nodes.AddRange( new[] { inventoryLevel1, inventoryLevel2, inventoryLevel3 } );

			// Act
			var result = productVariant.ToShopifyProductVariant();

			// Assert
			result.InventoryLevels.InventoryLevels.Should().HaveCount( 3 );
		}

		private static ProductVariant GetProductVariant()
		{
			var inventoryItemGid = "gid://shopify/InventoryItem/" + InventoryItemId;
			var productVariantGid = "gid://shopify/ProductVariant/" + ProductVariantId;
			return new ProductVariant()
			{
				Sku = Sku,
				InventoryItem = new InventoryItem() { InventoryItemId = inventoryItemGid },
				ProductVariantId = productVariantGid
			};
		}

		private static InventoryLevel GetInventoryLevel( int available, int locationId )
		{
			var locationGid = "gid://shopify/Location/" + locationId;
			return new InventoryLevel()
			{
				Quantities = CreateAvailableQuantity( available ),
				Location = new Location()
				{
					LocationId = locationGid
				}
			};
		}

		private static List< InventoryQuantity > CreateAvailableQuantity( int available )
		{
			return new List< InventoryQuantity >
			{
				new InventoryQuantity
				{
					Quantity = available
				}
			};
		}
	}
}