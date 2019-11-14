using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LINQtoCSV;
using Netco.Logging;
using Netco.Logging.NLogIntegration;
using NUnit.Framework;
using ShopifyAccess;
using ShopifyAccess.Models.Configuration.Command;

namespace ShopifyAccessTests.Users
{
	[ TestFixture ]
	public class UsersTests
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
		public void GetUsers()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			var users = service.GetUsers();

			users.Users.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetUsersAsync()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			var users = await service.GetUsersAsync( CancellationToken.None );

			users.Users.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetUser()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			var user = service.GetUser( 6250887 );

			user.Should().NotBeNull();
		}

		[ Test ]
		public async Task GetUserAsync()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			var user = await service.GetUserAsync( 6250887, CancellationToken.None );

			user.Should().NotBeNull();
		}

		[ Test ]
		public void DoesShopifyPlusAccount()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			var result = service.DoesShopifyPlusAccount();

			result.Should().Be( false );
		}

		[ Test ]
		public async Task DoesShopifyPlusAccountAsync()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			var result = await service.DoesShopifyPlusAccountAsync( CancellationToken.None );

			result.Should().Be( false );
		}
	}
}