using FluentAssertions;
using NUnit.Framework;
using ServiceStack.Text;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccessTests.Products.Models
{
	[ TestFixture ]
	public class ShopifyProductVariantTests
	{
		[ Test ]
		public void Deserialize_CorrectInventoryManagement_WhenDoesNotIgnoreCaseForEnum()
		{
			// Arrange
			var json = "{\"id\":1,\"inventory_management\":\"Shopify\",\"inventory_item_id\":0,\"weight\":0,\"price\":0,\"updated_at\":\"\\/Date(-62135596800000-0000)\\/\"}";

			// Act
			var shopifyProductVariant = JsonSerializer.DeserializeFromString< ShopifyProductVariant >( json );

			// Assert
			shopifyProductVariant.InventoryManagement.Should().Be( InventoryManagementEnum.Shopify );
		}

		[ Test ]
		public void Deserialize_CorrectInventoryManagement_WhenIgnoreCaseForEnum()
		{
			// Arrange
			var json = "{\"id\":1,\"inventory_management\":\"shopify\",\"inventory_item_id\":0,\"weight\":0,\"price\":0,\"updated_at\":\"\\/Date(-62135596800000-0000)\\/\"}";

			// Act
			var shopifyProductVariant = JsonSerializer.DeserializeFromString< ShopifyProductVariant >( json );

			// Assert
			shopifyProductVariant.InventoryManagement.Should().Be( InventoryManagementEnum.Shopify );
		}
	}
}