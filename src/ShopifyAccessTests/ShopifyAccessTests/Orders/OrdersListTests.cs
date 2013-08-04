using System;
using System.Linq;
using FluentAssertions;
using LINQtoCSV;
using NUnit.Framework;
using ShopifyAccess;
using ShopifyAccess.Exceptions;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Order;

namespace ShopifyAccessTests.Orders
{
	[ TestFixture ]
	public class OrdersListTests
	{
		private readonly IShopifyFactory ShopifyFactory = new ShopifyFactory();
		private ShopifyCommandConfig Config;

		[ SetUp ]
		public void Init()
		{
			const string credentialsFilePath = @"..\..\Files\ShopifyCredentials.csv";

			var cc = new CsvContext();
			var testConfig = cc.Read< TestCommandConfig >( credentialsFilePath, new CsvFileDescription { FirstLineHasColumnNames = true } ).FirstOrDefault();

			if( testConfig != null )
				this.Config = new ShopifyCommandConfig( testConfig.ShopName, testConfig.AccessToken );
		}

		[ Test ]
		public void OrdersFilteredByDateLoaded()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			var orders = service.GetOrders( DateTime.UtcNow.AddDays( -40 ), DateTime.UtcNow );

			orders.Count.Should().Be( 1 );
		}

		[ Test ]
		public void OrdersFilteredByDateLoadedAsync()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			var orders = service.GetOrdersAsync( DateTime.UtcNow.AddDays( -40 ), DateTime.UtcNow );

			orders.Result.Count.Should().Be( 1 );
		}

		[ Test ]
		public void OrdersFilteredFulfillmentStatusDateLoaded()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			var orders = service.GetOrders( ShopifyOrderFulfillmentStatus.any, DateTime.UtcNow.AddDays( -40 ), DateTime.UtcNow );

			orders.Count.Should().Be( 1 );
		}

		[ Test ]
		public void OrdersFilteredFulfillmentStatusDateLoadedAsync()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			var orders = service.GetOrdersAsync( ShopifyOrderFulfillmentStatus.any, DateTime.UtcNow.AddDays( -40 ), DateTime.UtcNow );

			orders.Result.Count.Should().Be( 1 );
		}

		[ Test ]
		public void OrdersNotLoaded_IncorrectToken()
		{
			var config = new ShopifyCommandConfig( this.Config.ShopName, "blabla" );
			var service = this.ShopifyFactory.CreateService( config );
			ShopifyOrders orders = null;
			try
			{
				orders = service.GetOrders( DateTime.UtcNow.AddDays( -40 ), DateTime.UtcNow );
			}
			catch( ShopifyException )
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
				orders = service.GetOrders( DateTime.UtcNow.AddDays( -40 ), DateTime.UtcNow );
			}
			catch( ShopifyException )
			{
				orders.Should().BeNull();
			}
		}
	}
}