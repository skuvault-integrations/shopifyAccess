using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ShopifyAccess.Models;

namespace ShopifyAccessTests.Cancellation
{
	[ TestFixture ]
	public class CancellationTokenTests : BaseTests
	{
		private static readonly Mark _mark = Mark.Create;
		
		[ Test ]
		public void CancelRequest()
		{
			var cancellationTokenSource = new CancellationTokenSource();

			Assert.ThrowsAsync< TaskCanceledException >( async () =>
			{
				cancellationTokenSource.Cancel();
				await this.Service.GetProductsCreatedAfterAsync( DateTime.UtcNow, cancellationTokenSource.Token, _mark );
				Assert.Fail();
			}, "Task wasn't cancelled" );
		}

		[ Test ]
		public void RequestTimesOut()
		{
			const int reallyShortTime = 1;
			var service = this.ShopifyFactory.CreateService( this._clientCredentials, new ShopifyTimeouts( reallyShortTime ) );
			var cancellationTokenSource = new CancellationTokenSource();

			Assert.ThrowsAsync< TaskCanceledException >( async () => 
			{
				await service.GetProductsCreatedAfterAsync( DateTime.UtcNow, cancellationTokenSource.Token, _mark );
			}, "Request didn't timeout. TaskCanceledException wasn't thrown");
		}
	}
}
