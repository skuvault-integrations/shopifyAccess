using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.Exceptions;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Order;

namespace ShopifyAccessTests.Orders
{
	[ Explicit ]
	[ TestFixture ]
	public class OrdersListTests : BaseTests
	{
		[ Test ]
		[ Explicit ]
		public async Task GetOrdersAsync_ReturnsItems_WhenAnyOrderStatusRequested()
		{
			var orders = await this.Service.GetOrdersAsync( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -200 ), DateTime.UtcNow, CancellationToken.None );

			orders.Count.Should().BeGreaterThan( 0 );
		}
		
		[ Test ]
		[ Explicit ]
		public async Task GetOrdersV2Async_ReturnsItems_WhenAnyOrderStatusRequested()
		{
			var orders = await this.Service.GetOrdersV2Async( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -200 ), DateTime.UtcNow, CancellationToken.None );

			orders.Count.Should().BeGreaterThan( 0 );
		}


		[ Test ]
		public async Task GetOrdersAsync_ThrowsShopifyUnauthorizedException_WhenIncorrectToken()
		{
			// Arrange
			var clientCredentials = new ShopifyClientCredentials( this._clientCredentials.ShopName, "blabla" );
			var service = this.ShopifyFactory.CreateService( clientCredentials );

			// Act, Assert
			await service.Invoking( s => s.GetOrdersAsync( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -200 ), DateTime.UtcNow, CancellationToken.None ) )
				.Should().ThrowAsync< ShopifyUnauthorizedException >();
		}

		[ Test ]
		public async Task GetOrdersAsync_ThrowsShopifyUnauthorizedException_WhenIncorrectShopName()
		{
			var clientCredentials = new ShopifyClientCredentials( "blabla", this._clientCredentials.AccessToken );
			var service = this.ShopifyFactory.CreateService( clientCredentials );

			// Act, Assert
			await service.Invoking( s => s.GetOrdersAsync( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -200 ), DateTime.UtcNow, CancellationToken.None ) )
				.Should().ThrowAsync< ShopifyUnauthorizedException >();
		}
	}
}