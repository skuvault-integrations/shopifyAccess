using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ShopifyAccess.Models;

namespace ShopifyAccessTests.Cancellation
{
	[ TestFixture ]
	public class CancellationTokenTests : BaseTests
	{
		[ Test ]
		public void CancelRequest()
		{
			var cancellationTokenSource = new CancellationTokenSource();

			Assert.ThrowsAsync< WebException >( async () =>
			{
				cancellationTokenSource.Cancel();
				await this.Service.GetProductsAsync( cancellationTokenSource.Token );
				Assert.Fail();
			}, "Task wasn't cancelled" );
		}

		[ Test ]
		public void RequestTimesOut()
		{
			const int reallyShortTime = 1;
			var service = this.ShopifyFactory.CreateService( this.Config, new ShopifyTimeouts( reallyShortTime ) );
			var cancellationTokenSource = new CancellationTokenSource();

			Assert.ThrowsAsync< TaskCanceledException >( async () => 
			{
				await service.GetProductsAsync( cancellationTokenSource.Token );
			}, "Request didn't timeout. TaskCanceledException wasn't thrown");
		}
	}
}
