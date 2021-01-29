using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Netco.Extensions;
using NUnit.Framework;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccessTests.Throttler
{
	[ TestFixture ]
	public class ThrottlerTests : BaseTests
	{
		[ Test ]
		public async Task ThrottlerTestAsync()
		{
			var list = new int[ 40 ];
			await list.DoInBatchAsync( 50, async x =>
			{
				var orders = await this.Service.GetOrdersAsync( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -20 ), DateTime.UtcNow, CancellationToken.None );
				var products = await this.Service.GetProductsAsync( CancellationToken.None );
				var variantToUpdate = new ShopifyProductVariantForUpdate { Id = 3341291969, Quantity = 2 };
				await this.Service.UpdateProductVariantsAsync( new List< ShopifyProductVariantForUpdate > { variantToUpdate }, CancellationToken.None );
			} );
		}

		[ Test ]
		public void ThrottlerTest()
		{
			var list = new int[ 40 ];
			list.DoInBatchAsync( 50, x =>
			{
				var task = new Task( () =>
				{
					var orders = this.Service.GetOrders( ShopifyOrderStatus.any, DateTime.UtcNow.AddDays( -20 ), DateTime.UtcNow, CancellationToken.None );
					var products = this.Service.GetProducts( CancellationToken.None );
					var variantToUpdate = new ShopifyProductVariantForUpdate { Id = 3341291969, Quantity = 2 };
					this.Service.UpdateProductVariants( new List< ShopifyProductVariantForUpdate > { variantToUpdate }, CancellationToken.None );
				} );
				task.Start();
				return task;
			} ).Wait();
		}
	}
}