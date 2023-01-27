using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
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
		public void OrdersNotLoaded_IncorrectToken()
		{
			var clientCredentials = new ShopifyClientCredentials( this._clientCredentials.ShopName, "blabla" );
			var service = this.ShopifyFactory.CreateService( clientCredentials );
			ShopifyOrders orders = null;
			try
			{
				orders = service.GetOrders( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -200 ), DateTime.UtcNow, CancellationToken.None );
			}
			catch( WebException )
			{
				orders.Should().BeNull();
			}
		}

		[ Test ]
		public void OrdersNotLoaded_IncorrectShopName()
		{
			var clientCredentials = new ShopifyClientCredentials( "blabla", this._clientCredentials.AccessToken );
			var service = this.ShopifyFactory.CreateService( clientCredentials );
			ShopifyOrders orders = null;
			try
			{
				orders = service.GetOrders( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -200 ), DateTime.UtcNow, CancellationToken.None );
			}
			catch( WebException )
			{
				orders.Should().BeNull();
			}
		}
	}
}