using FluentAssertions;
using NUnit.Framework;
using ServiceStack.Text;
using ShopifyAccess.Models.Order;

namespace ShopifyAccessTests.Orders.Models
{
	[ TestFixture ]
	public class ShopifyOrderTests
	{
		[ TestCase( "Fulfilled", FulfillmentStatusEnum.fulfilled ) ]
		[ TestCase( "Partial", FulfillmentStatusEnum.partial ) ]
		public void Deserialize_ReturnsFulfillmentStatus_IgnoringCase( string rawFulfillmentStatus, FulfillmentStatusEnum expectedFulfillmentStatus )
		{
			// Arrange
			var json = "{\"id\":0,\"total_price\":0,\"created_at\":\"\\/Date(-62135596800000-0000)\\/\",\"order_number\":0,\"financial_status\":\"Undefined\",\"fulfillment_status\":\"" + rawFulfillmentStatus + "\",\"source_name\":\"web\"}";

			// Act
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( json );

			// Assert
			shopifyOrder.FulfillmentStatus.Should().Be( expectedFulfillmentStatus );
		}

		[ TestCase( "" ) ]
		[ TestCase( null ) ]
		[ TestCase( "strange_status" ) ]
		public void Deserialize_ReturnsUndefined_WhenFulfillmentStatusIsNotValid( string rawFulfillmentStatus )
		{
			// Arrange
			var json = "{\"id\":0,\"total_price\":0,\"created_at\":\"\\/Date(-62135596800000-0000)\\/\",\"order_number\":0,\"financial_status\":\"Undefined\",\"fulfillment_status\":\"" + rawFulfillmentStatus + "\",\"source_name\":\"web\"}";

			// Act
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( json );

			// Assert
			shopifyOrder.FulfillmentStatus.Should().Be( FulfillmentStatusEnum.Undefined );
		}

		[ TestCase( "Web", ShopifySourceNameEnum.web ) ]
		[ TestCase( "Pos", ShopifySourceNameEnum.pos ) ]
		[ TestCase( "IPhone", ShopifySourceNameEnum.iphone ) ]
		[ TestCase( "Android", ShopifySourceNameEnum.android ) ]
		public void Deserialize_ReturnsSourceName_IgnoringCase( string rawSourceName, ShopifySourceNameEnum expectedSourceName )
		{
			// Arrange
			var json = "{\"id\":0,\"total_price\":0,\"created_at\":\"\\/Date(-62135596800000-0000)\\/\",\"order_number\":0,\"financial_status\":\"Undefined\",\"fulfillment_status\":\"\",\"source_name\":\"" + rawSourceName + "\"}";

			// Act
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( json );

			// Assert
			shopifyOrder.SourceName.Should().Be( expectedSourceName );
		}

		[ TestCase( "" ) ]
		[ TestCase( null ) ]
		[ TestCase( "strange_status" ) ]
		public void Deserialize_ReturnsUndefined_WhenSourceNameIsNotValid( string rawSourceName )
		{
			// Arrange
			var json = "{\"id\":0,\"total_price\":0,\"created_at\":\"\\/Date(-62135596800000-0000)\\/\",\"order_number\":0,\"financial_status\":\"Undefined\",\"fulfillment_status\":\"\",\"source_name\":\"" + rawSourceName + "\"}";

			// Act
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( json );

			// Assert
			shopifyOrder.SourceName.Should().Be( ShopifySourceNameEnum.Undefined );
		}
	}
}