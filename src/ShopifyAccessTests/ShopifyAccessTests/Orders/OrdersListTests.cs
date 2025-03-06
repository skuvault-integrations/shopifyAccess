using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.Models.Order;

namespace ShopifyAccessTests.Orders
{
	[ TestFixture ]
	public class OrdersListTests : BaseTests
	{
		//[Test]
		//[Explicit]
		//public async Task GetOrders_ReturnsItems_WhenAnyOrderStatusRequested()
		//{
		//	var orders = await this.Service.GetOrdersAsync(ShopifyOrderStatus.any, DateTime.UtcNow.AddDays(-200), DateTime.UtcNow, CancellationToken.None);

		//	orders.Count.Should().BeGreaterThan(0);
		//}

		[Test]
		[Explicit]
		public async Task GetOrdersAsync_ReturnsItems_WhenAnyOrderStatusRequested()
		{
			var dateStartLocal = new DateTime(2025, 1, 8, 22, 40, 0);
			var dateEndLocal = dateStartLocal.AddHours(1);
			var dateStart = dateStartLocal.ToUniversalTime();
			var dateEnd = dateEndLocal.ToUniversalTime();

			var orders = await this.Service.GetOrdersAsync(ShopifyOrderStatus.any, dateStart, dateEnd, CancellationToken.None);

			var order = orders.Orders.FirstOrDefault(f => f.Name == "LS24049389");
			var cancelledOrder = orders.Orders.FirstOrDefault(f => f.CancelledAt.HasValue);

			orders.Count.Should().BeGreaterThan(0);
		}

		//[ Test ]
		//[ Explicit ]
		//public void GetOrders_ThrowsShopifyUnauthorizedException_WhenIncorrectToken()
		//{
		//	// Arrange
		//	var clientCredentials = new ShopifyClientCredentials( this._clientCredentials.ShopName, "blabla" );
		//	var service = this.ShopifyFactory.CreateService( clientCredentials );

		//	// Act, Assert
		//	service.Invoking( s => s.GetOrders( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -200 ), DateTime.UtcNow, CancellationToken.None ) )
		//		.Should().Throw< ShopifyUnauthorizedException >();
		//}

		//[ Test ]
		//[ Explicit ]
		//public void GetOrders_ThrowsShopifyUnauthorizedException_WhenIncorrectShopName()
		//{
		//	var clientCredentials = new ShopifyClientCredentials( "blabla", this._clientCredentials.AccessToken );
		//	var service = this.ShopifyFactory.CreateService( clientCredentials );

		//	// Act, Assert
		//	service.Invoking( s => s.GetOrders( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -200 ), DateTime.UtcNow, CancellationToken.None ) )
		//		.Should().Throw< ShopifyUnauthorizedException >();
		//}
	}
}