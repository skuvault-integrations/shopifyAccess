using FluentAssertions;
using NUnit.Framework;
using ServiceStack.Text;
using ShopifyAccess.Models.Order;

namespace ShopifyAccessTests.Orders.Models
{
	[ TestFixture ]
	public class ShopifyOrderTests
	{
		[ TestCase( "" ) ]
		[ TestCase( null ) ]
		[ TestCase( "Undefined" ) ]
		[ TestCase( "undefined" ) ]
		[ TestCase( "strange_status" ) ]
		public void Deserialize_ReturnsUndefinedFulfillmentStatus( string rawFulfillmentStatus )
		{
			// Arrange
			var json = "{\"id\":0,\"total_price\":0,\"created_at\":\"\\/Date(-62135596800000-0000)\\/\",\"order_number\":0,\"financial_status\":\"Undefined\",\"fulfillment_status\":\"" + rawFulfillmentStatus + "\",\"source_name\":\"web\"}";

			// Act
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( json );

			// Assert
			shopifyOrder.FulfillmentStatus.Should().Be( FulfillmentStatusEnum.Undefined );
		}

		[ TestCase( "Fulfilled" ) ]
		[ TestCase( "fulfilled" ) ]
		public void Deserialize_ReturnsFulfilledFulfillmentStatus( string rawFulfillmentStatus )
		{
			// Arrange
			var json = "{\"id\":0,\"total_price\":0,\"created_at\":\"\\/Date(-62135596800000-0000)\\/\",\"order_number\":0,\"financial_status\":\"Undefined\",\"fulfillment_status\":\"" + rawFulfillmentStatus + "\",\"source_name\":\"web\"}";

			// Act
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( json );

			// Assert
			shopifyOrder.FulfillmentStatus.Should().Be( FulfillmentStatusEnum.fulfilled );
		}

		[ TestCase( "Partial" ) ]
		[ TestCase( "partial" ) ]
		public void Deserialize_ReturnsPartialFulfillmentStatus( string rawFulfillmentStatus )
		{
			// Arrange
			var json = "{\"id\":0,\"total_price\":0,\"created_at\":\"\\/Date(-62135596800000-0000)\\/\",\"order_number\":0,\"financial_status\":\"Undefined\",\"fulfillment_status\":\"" + rawFulfillmentStatus + "\",\"source_name\":\"web\"}";

			// Act
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( json );

			// Assert
			shopifyOrder.FulfillmentStatus.Should().Be( FulfillmentStatusEnum.partial );
		}

		[ TestCase( "" ) ]
		[ TestCase( null ) ]
		[ TestCase( "Undefined" ) ]
		[ TestCase( "undefined" ) ]
		[ TestCase( "strange_status" ) ]
		public void Deserialize_ReturnsUndefinedSourceName( string rawSourceName )
		{
			// Arrange
			var json = "{\"id\":0,\"total_price\":0,\"created_at\":\"\\/Date(-62135596800000-0000)\\/\",\"order_number\":0,\"financial_status\":\"Undefined\",\"fulfillment_status\":\"\",\"source_name\":\"" + rawSourceName + "\"}";

			// Act
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( json );

			// Assert
			shopifyOrder.SourceName.Should().Be( ShopifySourceNameEnum.Undefined );
		}

		[ TestCase( "Web" ) ]
		[ TestCase( "web" ) ]
		public void Deserialize_ReturnsWebSourceName( string rawSourceName )
		{
			// Arrange
			var json = "{\"id\":0,\"total_price\":0,\"created_at\":\"\\/Date(-62135596800000-0000)\\/\",\"order_number\":0,\"financial_status\":\"Undefined\",\"fulfillment_status\":\"\",\"source_name\":\"" + rawSourceName + "\"}";

			// Act
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( json );

			// Assert
			shopifyOrder.SourceName.Should().Be( ShopifySourceNameEnum.web );
		}

		[ TestCase( "Pos" ) ]
		[ TestCase( "pos" ) ]
		public void Deserialize_ReturnsPosSourceName( string rawSourceName )
		{
			// Arrange
			var json = "{\"id\":0,\"total_price\":0,\"created_at\":\"\\/Date(-62135596800000-0000)\\/\",\"order_number\":0,\"financial_status\":\"Undefined\",\"fulfillment_status\":\"\",\"source_name\":\"" + rawSourceName + "\"}";

			// Act
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( json );

			// Assert
			shopifyOrder.SourceName.Should().Be( ShopifySourceNameEnum.pos );
		}

		[ TestCase( "IPhone" ) ]
		[ TestCase( "iphone" ) ]
		public void Deserialize_ReturnsIPhoneSourceName( string rawSourceName )
		{
			// Arrange
			var json = "{\"id\":0,\"total_price\":0,\"created_at\":\"\\/Date(-62135596800000-0000)\\/\",\"order_number\":0,\"financial_status\":\"Undefined\",\"fulfillment_status\":\"\",\"source_name\":\"" + rawSourceName + "\"}";

			// Act
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( json );

			// Assert
			shopifyOrder.SourceName.Should().Be( ShopifySourceNameEnum.iphone );
		}

		[ TestCase( "Android" ) ]
		[ TestCase( "android" ) ]
		public void Deserialize_ReturnsAndroidSourceName( string rawSourceName )
		{
			// Arrange
			var json = "{\"id\":0,\"total_price\":0,\"created_at\":\"\\/Date(-62135596800000-0000)\\/\",\"order_number\":0,\"financial_status\":\"Undefined\",\"fulfillment_status\":\"\",\"source_name\":\"" + rawSourceName + "\"}";

			// Act
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( json );

			// Assert
			shopifyOrder.SourceName.Should().Be( ShopifySourceNameEnum.android );
		}
	}
}