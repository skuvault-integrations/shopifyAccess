﻿using System;
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
		public void OrdersFilteredFulfillmentStatusDateLoaded()
		{
			var orders = this.Service.GetOrders( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -200), DateTime.UtcNow, CancellationToken.None );

			orders.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task OrdersFilteredFulfillmentStatusDateLoadedAsync()
		{
			var orders = await this.Service.GetOrdersAsync( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -200 ), DateTime.UtcNow, CancellationToken.None );

			orders.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void OrdersNotLoaded_IncorrectToken()
		{
			var config = new ShopifyCommandConfig( this.Config.ShopName, "blabla" );
			var service = this.ShopifyFactory.CreateService( config );
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
			var config = new ShopifyCommandConfig( "blabla", this.Config.AccessToken );
			var service = this.ShopifyFactory.CreateService( config );
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