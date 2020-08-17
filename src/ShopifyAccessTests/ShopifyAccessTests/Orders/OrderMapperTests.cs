using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.Order.Discounts;

namespace ShopifyAccessTests.Orders
{
	[ TestFixture ]
	public class OrderMapperTests
	{
		[ Test ]
		public void ShopifyDiscountCode_TypeIsMapped()
		{
			var discountCodeFixed = new ShopifyDiscountCode( "fixed_amount" );
			var discountCodePercentage = new ShopifyDiscountCode( "percentage" );
			var discountCodeUndefined = new ShopifyDiscountCode( "some unknown type" );

			discountCodeFixed.Type.Should().Be( ShopifyDiscountTypeEnum.FixedAmount );
			discountCodePercentage.Type.Should().Be( ShopifyDiscountTypeEnum.Percentage );
			discountCodeUndefined.Type.Should().Be( ShopifyDiscountTypeEnum.Undefined );
		}

		[ Test ]
		public void ShopifyFulfillmentStatus_TypeIsMapped()
		{
			var fulfillmentStatusUnknown = new ShopifyFulfillment( "some_new_status" );
			var fulfillmentStatusPending = new ShopifyFulfillment( "pending" );
			var fulfillmentStatusOpen = new ShopifyFulfillment( "open" );
			var fulfillmentStatusSuccess = new ShopifyFulfillment( "success" );
			var fulfillmentStatusCancelled = new ShopifyFulfillment( "cancelled" );
			var fulfillmentStatusError = new ShopifyFulfillment( "error" );
			var fulfillmentStatusFailure = new ShopifyFulfillment( "failure" );

			fulfillmentStatusUnknown.Status.Should().Be( ShopifyOrderFulfillmentStatusEnum.Undefined );
			fulfillmentStatusPending.Status.Should().Be( ShopifyOrderFulfillmentStatusEnum.Pending );
			fulfillmentStatusOpen.Status.Should().Be( ShopifyOrderFulfillmentStatusEnum.Open );
			fulfillmentStatusSuccess.Status.Should().Be( ShopifyOrderFulfillmentStatusEnum.Success );
			fulfillmentStatusCancelled.Status.Should().Be( ShopifyOrderFulfillmentStatusEnum.Cancelled );
			fulfillmentStatusError.Status.Should().Be( ShopifyOrderFulfillmentStatusEnum.Error );
			fulfillmentStatusFailure.Status.Should().Be( ShopifyOrderFulfillmentStatusEnum.Failure );
		}
	}
}
