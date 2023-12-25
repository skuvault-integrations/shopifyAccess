using FluentAssertions;
using NUnit.Framework;
using ServiceStack.Text;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccessTests.Products.Models
{
	[ TestFixture ]
	public class ShopifyProductVariantTests
	{
		[ TestCase( "" ) ]
		[ TestCase( null ) ]
		[ TestCase( "Blank" ) ]
		[ TestCase( "blank" ) ]
		[ TestCase( "strange_status" ) ]
		public void Deserialize_ReturnsBlankInventoryManagement( string rawInventoryManagement )
		{
			// Arrange
			var json = "{\"id\":1,\"inventory_management\":\"" + rawInventoryManagement + "\",\"inventory_item_id\":0,\"weight\":0,\"price\":0,\"updated_at\":\"\\/Date(-62135596800000-0000)\\/\"}";

			// Act
			var shopifyProductVariant = JsonSerializer.DeserializeFromString< ShopifyProductVariant >( json );

			// Assert
			shopifyProductVariant.InventoryManagement.Should().Be( InventoryManagementEnum.Blank );
		}

		[ TestCase( "Shopify" ) ]
		[ TestCase( "shopify" ) ]
		public void Deserialize_ReturnsShopifyInventoryManagement( string rawInventoryManagement )
		{
			// Arrange
			var json = "{\"id\":1,\"inventory_management\":\"" + rawInventoryManagement + "\",\"inventory_item_id\":0,\"weight\":0,\"price\":0,\"updated_at\":\"\\/Date(-62135596800000-0000)\\/\"}";

			// Act
			var shopifyProductVariant = JsonSerializer.DeserializeFromString< ShopifyProductVariant >( json );

			// Assert
			shopifyProductVariant.InventoryManagement.Should().Be( InventoryManagementEnum.Shopify );
		}
	}
}