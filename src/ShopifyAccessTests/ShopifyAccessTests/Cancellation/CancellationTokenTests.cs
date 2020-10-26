using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LINQtoCSV;
using Netco.Logging;
using NUnit.Framework;
using ShopifyAccess;
using ShopifyAccess.Models;
using ShopifyAccess.Models.Configuration.Command;

namespace ShopifyAccessTests.Cancellation
{
	[ TestFixture ]
	public class CancellationTokenTests
	{
		private readonly IShopifyFactory ShopifyFactory = new ShopifyFactory();
		private TestCommandConfig TestConfig;

		[ SetUp ]
		public void Init()
		{
			Directory.SetCurrentDirectory( TestContext.CurrentContext.TestDirectory );
			const string credentialsFilePath = @"..\..\Files\ShopifyCredentials.csv";
			NetcoLogger.LoggerFactory = new ConsoleLoggerFactory();

			var cc = new CsvContext();
			this.TestConfig = cc.Read< TestCommandConfig >( credentialsFilePath, new CsvFileDescription { FirstLineHasColumnNames = true } ).FirstOrDefault();
		}

		private ShopifyCommandConfig CreateConfig()
		{
			if( this.TestConfig == null )
				return null;

			return new ShopifyCommandConfig( this.TestConfig.ShopName, this.TestConfig.AccessToken );
		}

		[ Test ]
		public void CancelRequest()
		{
			var service = this.ShopifyFactory.CreateService( this.CreateConfig() );
			var cancellationTokenSource = new CancellationTokenSource();

			Assert.ThrowsAsync< WebException >( async () =>
			{
				cancellationTokenSource.Cancel();
				await service.GetProductsAsync( cancellationTokenSource.Token );
				Assert.Fail();
			}, "Task wasn't cancelled" );
		}

		[ Test ]
		public void RequestTimesOut()
		{
			const int reallyShortTime = 1;
			var service = this.ShopifyFactory.CreateService( this.CreateConfig(), new ShopifyTimeouts( reallyShortTime ) );
			var cancellationTokenSource = new CancellationTokenSource();

			Assert.ThrowsAsync< TaskCanceledException >( async () => 
			{
				await service.GetProductsAsync( cancellationTokenSource.Token );
			}, "Request didn't timeout. TaskCanceledException wasn't thrown");
		}
	}
}
