using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess;
using ShopifyAccess.Models.Core.Configuration;

namespace ShopifyAccessTests.Orders
{
	[ TestFixture ]
	public class OrdersListTests
	{
		private readonly IShopifyFactory ShopifyFactory = new ShopifyFactory();
		private const string ShopName = "skuvault";
		private const string AccessToken = "ce22522b5b2ad8cce975429ec265db4c";

		[ Test ]
		public void OrdersLoaded()
		{
			var config = new ShopifyCommandConfig( ShopName, AccessToken );
			var service = this.ShopifyFactory.CreateService( config );
			var orders = service.GetOrders();

			orders.Count.Should().Be( 1 );
		}

		[ Test ]
		public void OrdersLoadedAsync()
		{
			var config = new ShopifyCommandConfig( ShopName, AccessToken );
			var service = this.ShopifyFactory.CreateService( config );
			var orders = service.GetOrdersAsync();

			orders.Result.Count.Should().Be( 1 );
		}

		[ Test ]
		public void OrdersNotLoaded_IncorrectToken()
		{
			var config = new ShopifyCommandConfig( ShopName, "blabla" );
			var service = this.ShopifyFactory.CreateService( config );
			var orders = service.GetOrders();

			orders.Should().BeNull();
		}

		[ Test ]
		public void OrdersNotLoaded_IncorrectShopName()
		{
			var config = new ShopifyCommandConfig( "blabla", AccessToken );
			var service = this.ShopifyFactory.CreateService( config );
			var orders = service.GetOrders();

			orders.Should().BeNull();
		}
	}
}