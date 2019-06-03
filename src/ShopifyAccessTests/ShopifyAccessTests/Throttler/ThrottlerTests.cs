using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LINQtoCSV;
using Netco.Extensions;
using Netco.Logging;
using NUnit.Framework;
using ShopifyAccess;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccessTests.Throttler
{
	[ TestFixture ]
	public class ThrottlerTests
	{
		private readonly IShopifyFactory ShopifyFactory = new ShopifyFactory();
		private ShopifyCommandConfig Config;

		[ SetUp ]
		public void Init()
		{
			Directory.SetCurrentDirectory( TestContext.CurrentContext.TestDirectory );
			const string credentialsFilePath = @"..\..\Files\ShopifyCredentials.csv";
			NetcoLogger.LoggerFactory = new ConsoleLoggerFactory();

			var cc = new CsvContext();
			var testConfig = cc.Read< TestCommandConfig >( credentialsFilePath, new CsvFileDescription { FirstLineHasColumnNames = true } ).FirstOrDefault();

			if( testConfig != null )
				this.Config = new ShopifyCommandConfig( testConfig.ShopName, testConfig.AccessToken );
		}

		[ Test ]
		public async Task ThrottlerTestAsync()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );

			var list = new int[ 40 ];
			await list.DoInBatchAsync( 50, async x =>
			{
				var orders = await service.GetOrdersAsync( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -20 ), DateTime.UtcNow );
				var products = await service.GetProductsAsync();
				var variantToUpdate = new ShopifyProductVariantForUpdate { Id = 3341291969, Quantity = 2 };
				await service.UpdateProductVariantsAsync( new List< ShopifyProductVariantForUpdate > { variantToUpdate } );
			} );
		}

		[ Test ]
		public void ThrottlerTest()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );

			var list = new int[ 40 ];
			list.DoInBatchAsync( 50, x =>
			{
				var task = new Task( () =>
				{
					var orders = service.GetOrders( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -20 ), DateTime.UtcNow );
					var products = service.GetProducts();
					var variantToUpdate = new ShopifyProductVariantForUpdate { Id = 3341291969, Quantity = 2 };
					service.UpdateProductVariants( new List< ShopifyProductVariantForUpdate > { variantToUpdate } );
				} );
				task.Start();
				return task;
			} ).Wait();
		}
	}
}