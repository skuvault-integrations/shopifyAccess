using FluentAssertions;
using NUnit.Framework;
using ServiceStack.Text;
using ShopifyAccess.Models.Order;

namespace ShopifyAccessTests.Orders.Models
{
	[ TestFixture ]
	public class ShopifyOrderTests
	{
		[ TestCase( "", FulfillmentStatusEnum.Undefined ) ]
		[ TestCase( null, FulfillmentStatusEnum.Undefined ) ]
		[ TestCase( "Undefined", FulfillmentStatusEnum.Undefined ) ]
		[ TestCase( "undefined", FulfillmentStatusEnum.Undefined ) ]
		[ TestCase( "strange_status", FulfillmentStatusEnum.Undefined ) ]
		[ TestCase( "Fulfilled", FulfillmentStatusEnum.fulfilled ) ]
		[ TestCase( "fulfilled", FulfillmentStatusEnum.fulfilled ) ]
		[ TestCase( "Partial", FulfillmentStatusEnum.partial ) ]
		[ TestCase( "partial", FulfillmentStatusEnum.partial ) ]
		public void Deserialize_ReturnsFulfillmentStatus_IgnoringCase( string rawFulfillmentStatus, FulfillmentStatusEnum expectedFulfillmentStatus )
		{
			// Arrange
			var json = "{\"id\":0,\"total_price\":0,\"created_at\":\"\\/Date(-62135596800000-0000)\\/\",\"order_number\":0,\"financial_status\":\"Undefined\",\"fulfillment_status\":\"" + rawFulfillmentStatus + "\",\"source_name\":\"web\"}";

			// Act
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( json );

			// Assert
			shopifyOrder.FulfillmentStatus.Should().Be( expectedFulfillmentStatus );
		}

		[ TestCase( "", ShopifySourceNameEnum.Undefined ) ]
		[ TestCase( null, ShopifySourceNameEnum.Undefined ) ]
		[ TestCase( "Undefined", ShopifySourceNameEnum.Undefined ) ]
		[ TestCase( "undefined", ShopifySourceNameEnum.Undefined ) ]
		[ TestCase( "strange_status", ShopifySourceNameEnum.Undefined ) ]
		[ TestCase( "Web", ShopifySourceNameEnum.web ) ]
		[ TestCase( "web", ShopifySourceNameEnum.web ) ]
		[ TestCase( "Pos", ShopifySourceNameEnum.pos ) ]
		[ TestCase( "pos", ShopifySourceNameEnum.pos ) ]
		[ TestCase( "IPhone", ShopifySourceNameEnum.iphone ) ]
		[ TestCase( "iphone", ShopifySourceNameEnum.iphone ) ]
		[ TestCase( "Android", ShopifySourceNameEnum.android ) ]
		[ TestCase( "android", ShopifySourceNameEnum.android ) ]
		public void Deserialize_ReturnsUndefinedSourceName( string rawSourceName, ShopifySourceNameEnum expectedShopifySourceName )
		{
			// Arrange
			var json = "{\"id\":0,\"total_price\":0,\"created_at\":\"\\/Date(-62135596800000-0000)\\/\",\"order_number\":0,\"financial_status\":\"Undefined\",\"fulfillment_status\":\"\",\"source_name\":\"" + rawSourceName + "\"}";

			// Act
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( json );

			// Assert
			shopifyOrder.SourceName.Should().Be( expectedShopifySourceName );
		}
	}
}