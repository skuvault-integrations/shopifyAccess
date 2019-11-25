using FluentAssertions;
using NUnit.Framework;
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
	}
}
