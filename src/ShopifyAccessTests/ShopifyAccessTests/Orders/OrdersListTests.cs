using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.Exceptions;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Order;

namespace ShopifyAccessTests.Orders
{
	[ TestFixture ]
	public class OrdersListTests : BaseTests
	{
		[ Test ]
		[ Explicit ]
		public void GetOrders_ReturnsItems_WhenAnyOrderStatusRequested()
		{
			var orders = this.Service.GetOrders( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -200 ), DateTime.UtcNow, CancellationToken.None );

			orders.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetOrdersAsync_ReturnsItems_WhenAnyOrderStatusRequested()
		{
			var orders = await this.Service.GetOrdersAsync( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -200 ), DateTime.UtcNow, CancellationToken.None );

			orders.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetOrders_ThrowsShopifyUnauthorizedException_WhenIncorrectToken()
		{
			// Arrange
			var clientCredentials = new ShopifyClientCredentials( this._clientCredentials.ShopName, "blabla" );
			var service = this.ShopifyFactory.CreateService( clientCredentials );

			// Act, Assert
			service.Invoking( s => s.GetOrders( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -200 ), DateTime.UtcNow, CancellationToken.None ) )
				.Should().Throw< ShopifyUnauthorizedException >();
		}

		[ Test ]
		public void GetOrders_ThrowsShopifyUnauthorizedException_WhenIncorrectShopName()
		{
			var clientCredentials = new ShopifyClientCredentials( "blabla", this._clientCredentials.AccessToken );
			var service = this.ShopifyFactory.CreateService( clientCredentials );

			// Act, Assert
			service.Invoking( s => s.GetOrders( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -200 ), DateTime.UtcNow, CancellationToken.None ) )
				.Should().Throw< HttpRequestException >();
		}
	}
}