using System;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess;
using ShopifyAccess.Models.Core.Configuration.Command;

namespace ShopifyAccessTests.Orders
{
	[ TestFixture ]
	public class OrdersListTests
	{
		private readonly IShopifyFactory ShopifyFactory = new ShopifyFactory();
		private const string ShopName = "skuvault";
		private const string AccessToken = "ce22522b5b2ad8cce975429ec265db4c";

		[ Test ]
		public void OrdersFilteredByDateLoaded()
		{
			var config = new ShopifyCommandConfig( ShopName, AccessToken );
			var service = this.ShopifyFactory.CreateService( config );
			var orders = service.GetOrders( DateTime.UtcNow.AddDays( -40 ), DateTime.UtcNow );

			orders.Count.Should().Be( 1 );
		}

		[ Test ]
		public void OrdersFilteredByDateLoadedAsync()
		{
			var config = new ShopifyCommandConfig( ShopName, AccessToken );
			var service = this.ShopifyFactory.CreateService( config );
			var orders = service.GetOrdersAsync( DateTime.UtcNow.AddDays( -40 ), DateTime.UtcNow );

			orders.Result.Count.Should().Be( 1 );
		}

		[ Test ]
		public void OrdersFilteredFulfillmentStatusDateLoaded()
		{
			var config = new ShopifyCommandConfig( ShopName, AccessToken );
			var service = this.ShopifyFactory.CreateService( config );
			var orders = service.GetOrders( ShopifyOrderFulfillmentStatus.any );

			orders.Count.Should().Be( 1 );
		}

		[ Test ]
		public void OrdersFilteredFulfillmentStatusDateLoadedAsync()
		{
			var config = new ShopifyCommandConfig( ShopName, AccessToken );
			var service = this.ShopifyFactory.CreateService( config );
			var orders = service.GetOrdersAsync( ShopifyOrderFulfillmentStatus.any );

			orders.Result.Count.Should().Be( 1 );
		}

		[ Test ]
		public void OrdersNotLoaded_IncorrectToken()
		{
			var config = new ShopifyCommandConfig( ShopName, "blabla" );
			var service = this.ShopifyFactory.CreateService( config );
			var orders = service.GetOrders( DateTime.UtcNow.AddDays( -40 ), DateTime.UtcNow );

			orders.Should().BeNull();
		}

		[ Test ]
		public void OrdersNotLoaded_IncorrectShopName()
		{
			var config = new ShopifyCommandConfig( "blabla", AccessToken );
			var service = this.ShopifyFactory.CreateService( config );
			var orders = service.GetOrders( DateTime.UtcNow.AddDays( -40 ), DateTime.UtcNow );

			orders.Should().BeNull();
		}
	}
}