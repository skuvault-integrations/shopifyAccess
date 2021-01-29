using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.Models;

namespace ShopifyAccessTests
{
	[ TestFixture ]
	public class OperationTimeoutTests
	{
		[ Test ]
		public void GivenSpecificTimeoutsAreNotSet_WhenGetTimeoutIsCalled_ThenDefaultTimeoutIsReturned()
		{
			var operationsTimeouts = new ShopifyTimeouts();

			operationsTimeouts[ ShopifyOperationEnum.GetOrders ].Should().Be( operationsTimeouts.DefaultOperationTimeout.TimeoutInMs );
		}

		[ Test ]
		public void GivenDefaultTimeoutValuePassedIntoConstructor_WhenGetTimeoutIsCalled_ThenOverridenDefaultTimeoutIsReturned()
		{
			const int newDefaultTimeoutInMs = 10 * 60 * 1000;
			var operationsTimeouts = new ShopifyTimeouts( newDefaultTimeoutInMs );

			operationsTimeouts[ ShopifyOperationEnum.GetProducts ].Should().Be( newDefaultTimeoutInMs );
		}

		[ Test ]
		public void GivenGetOrdersTimeoutIsSet_WhenGetTimeoutIsCalled_ThenSpecificTimeoutIsReturned()
		{
			var operationsTimeouts = new ShopifyTimeouts();
			const int specificTimeoutInMs = 10 * 60 * 1000;
			operationsTimeouts.Set( ShopifyOperationEnum.GetOrders, new ShopifyOperationTimeout( specificTimeoutInMs ) );

			operationsTimeouts[ ShopifyOperationEnum.GetOrders ].Should().Be( specificTimeoutInMs );
			operationsTimeouts[ ShopifyOperationEnum.GetProductsCount ].Should().Be( operationsTimeouts.DefaultOperationTimeout.TimeoutInMs );
		}

		[Test]
		public void GivenGetOrdersTimeoutIsSetTwice_WhenGetTimeoutIsCalled_ThenSpecificTimeoutIsReturned()
		{
			var operationsTimeouts = new ShopifyTimeouts();
			const int specificTimeoutInMs = 10 * 60 * 1000;
			operationsTimeouts.Set( ShopifyOperationEnum.GetOrders, new ShopifyOperationTimeout( specificTimeoutInMs ) );
			operationsTimeouts.Set( ShopifyOperationEnum.GetOrders, new ShopifyOperationTimeout( specificTimeoutInMs * 2 ) );

			operationsTimeouts [ShopifyOperationEnum.GetOrders].Should().Be( specificTimeoutInMs * 2 );
		}
	}
}
