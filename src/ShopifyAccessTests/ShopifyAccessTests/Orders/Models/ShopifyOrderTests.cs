using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using ShopifyAccess.GraphQl.Models.Orders;
using ShopifyAccess.Models.Order;

namespace ShopifyAccessTests.Orders.Models
{
	[ TestFixture ]
	public class ShopifyOrderTests
	{
		[ TestCase( "Fulfilled", FulfillmentStatusEnum.fulfilled ) ]
		public void Deserialize_ReturnsFulfillmentStatus_IgnoringCase( string rawFulfillmentStatus, FulfillmentStatusEnum expectedFulfillmentStatus )
		{
			// Arrange
			var json = "{ \"id\": \"gid://shopify/Order/6026305732922\", \"number\": 1024, \"createdAt\": \"2024-07-12T17:14:07Z\", \"totalPriceSet\": { \"shopMoney\": { \"amount\": \"0.0\", \"currencyCode\": \"USD\" } }, \"displayFulfillmentStatus\": \" " + rawFulfillmentStatus + " \", \"fulfillments\": [], \"sourceName\": \" web \" }";

			// Act
			var order = JsonConvert.DeserializeObject< Order >( json );

			// Assert
			order.FulfillmentStatus.Should().Be( expectedFulfillmentStatus );
		}

		[ TestCase( "" ) ]
		[ TestCase( null ) ]
		[ TestCase( "strange_status" ) ]
		public void Deserialize_ReturnsUndefined_WhenFulfillmentStatusIsNotValid( string rawFulfillmentStatus )
		{
			// Arrange
			var json = "{ \"id\": \"gid://shopify/Order/6026305732922\", \"number\": 1024, \"createdAt\": \"2024-07-12T17:14:07Z\", \"totalPriceSet\": { \"shopMoney\": { \"amount\": \"0.0\", \"currencyCode\": \"USD\" } }, \"displayFulfillmentStatus\": \" " + rawFulfillmentStatus + " \", \"fulfillments\": [], \"sourceName\": \" web \" }";

			// Act
			var order = JsonConvert.DeserializeObject< Order >( json );

			// Assert
			order.FulfillmentStatus.Should().Be( FulfillmentStatusEnum.Undefined );
		}

		[ TestCase( "Web", ShopifySourceNameEnum.web ) ]
		public void Deserialize_ReturnsSourceName_IgnoringCase( string rawSourceName, ShopifySourceNameEnum expectedSourceName )
		{
			// Arrange
			var json = "{ \"id\": \"gid://shopify/Order/6026305732922\", \"number\": 1024, \"createdAt\": \"2024-07-12T17:14:07Z\", \"totalPriceSet\": { \"shopMoney\": { \"amount\": \"0.0\", \"currencyCode\": \"USD\" } }, \"displayFinancialStatus\": \"PAID\", \"fulfillments\": [], \"sourceName\": \" " + rawSourceName + "\" }";

			// Act
			var order = JsonConvert.DeserializeObject< Order >( json );

			// Assert
			order.SourceName.Should().Be( expectedSourceName );
		}

		[ TestCase( "" ) ]
		[ TestCase( null ) ]
		[ TestCase( "strange_status" ) ]
		public void Deserialize_ReturnsUndefined_WhenSourceNameIsNotValid( string rawSourceName )
		{
			// Arrange
			var json = "{ \"id\": \"gid://shopify/Order/6026305732922\", \"number\": 1024, \"createdAt\": \"2024-07-12T17:14:07Z\", \"totalPriceSet\": { \"shopMoney\": { \"amount\": \"0.0\", \"currencyCode\": \"USD\" } }, \"displayFinancialStatus\": \"PAID\", \"fulfillments\": [], \"sourceName\": \" " + rawSourceName + "\" }";

			// Act
			var order = JsonConvert.DeserializeObject< Order >( json );

			// Assert
			order.SourceName.Should().Be( ShopifySourceNameEnum.Undefined );
		}
		
		// TODO GUARD-3910 Delete on feature cleanup. No longer relevant since with GraphQl we don't use ShopifyOrder for deserialization
		[ Test ]
		public void DeserializeShopifyOrder_ReturnsUndefinedShopifyFinancialStatus_WhenApiReturnsNullFinancialStatus()
		{
			const string serializedShopifyOrder = @"{ ""financial_status"" : null }";

			var result = JsonConvert.DeserializeObject< ShopifyOrder >( serializedShopifyOrder );

			Assert.That( result.FinancialStatus, Is.EqualTo( ShopifyFinancialStatus.Undefined ) );
		}

		[ Test ]
		public void DeserializeOrder_ReturnsUndefinedShopifyFinancialStatus_WhenApiReturnsNullFinancialStatus()
		{
			const string serializedOrder = @"{ ""displayFinancialStatus"" : null }";

			var result = JsonConvert.DeserializeObject< Order >( serializedOrder );

			Assert.That( result.FinancialStatus, Is.EqualTo( ShopifyFinancialStatus.Undefined ) );
		}
	}
}
