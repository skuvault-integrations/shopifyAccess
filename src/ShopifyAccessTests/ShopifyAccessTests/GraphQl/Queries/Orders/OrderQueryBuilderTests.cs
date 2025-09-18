using System;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ShopifyAccess.GraphQl.Helpers;
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

		[ TestCase(
			"2025-09-17 20:56:05", "2025-09-17 20:59:37",
			"2025-09-17T20:56:05.0000000Z", "2025-09-17T20:59:37.0000000Z"
		) ]
		public void GetOrdersRequest_ShouldIncludeUtcIso8601Dates( string inputFrom, string inputTo, string expectedFrom, string expectedTo )
		{
			// Arrange
			var dateFrom = DateTime.SpecifyKind( DateTime.Parse( inputFrom ), DateTimeKind.Utc );
			var dateTo = DateTime.SpecifyKind( DateTime.Parse( inputTo ), DateTimeKind.Utc );

			// Act
			var query = OrderQueryBuilder.GetOrdersRequest(
				dateFrom,
				dateTo,
				status : ShopifyOrderStatus.any.ToString(),
				after : null,
				ordersPerPage : 10
			);

			var formattedFrom = dateFrom.ToIso8601();
			var formattedTo = dateTo.ToIso8601();

			// Assert
			Assert.That( formattedFrom, Does.StartWith( expectedFrom ) );
			Assert.That( formattedFrom, Does.EndWith( "Z" ) );
			Assert.That( formattedTo, Does.StartWith( expectedTo ) );
			Assert.That( formattedTo, Does.EndWith( "Z" ) );

			Assert.That( query, Does.Contain( formattedFrom ) );
			Assert.That( query, Does.Contain( formattedTo ) );
		}
	}
}
