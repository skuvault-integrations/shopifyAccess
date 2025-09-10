using System;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ShopifyAccess.GraphQl.Queries.Orders;
using ShopifyAccess.Models.Order;

namespace ShopifyAccessTests.GraphQl.Queries.Orders
{
	public class OrderQueryBuilderTests
	{
		private static readonly Randomizer _randomizer = new Randomizer();

		[ Test ]
		public void GetOrders_ThrowsArgumentOutOfRangeException_WhenOrdersPerPageExceedsMaximum()
		{
			var tooManyOrdersPerPage = 251;

			Assert.Throws< ArgumentOutOfRangeException >( () => OrderQueryBuilder.GetOrdersRequest( DateTime.UtcNow.AddDays( -1 ), DateTime.UtcNow, status : ShopifyOrderStatus.any.ToString(), after : null, tooManyOrdersPerPage ) );
		}

		[ Test ]
		public void GetOrders_ReturnsRequestQuery()
		{
			var nextCursor = _randomizer.GetString();
			var ordersPerPage = ( int )_randomizer.NextUInt( 1, 250 );

			var result = OrderQueryBuilder.GetOrdersRequest( DateTime.UtcNow.AddDays( -1 ), DateTime.UtcNow, status : ShopifyOrderStatus.any.ToString(), after : nextCursor, ordersPerPage );

			Assert.Multiple( () =>
			{
				Assert.That( result.Contains( nextCursor ), Is.True );
				Assert.That( result.Contains( ordersPerPage.ToString() ), Is.True );
			} );
		}
	}
}
