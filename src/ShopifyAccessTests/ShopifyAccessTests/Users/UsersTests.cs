using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace ShopifyAccessTests.Users
{
	[ TestFixture ]
	public class UsersTests : BaseTests
	{
		[ Test ]
		public void GetUsers()
		{
			var users = this.Service.GetUsers( CancellationToken.None );

			users.Users.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetUsersAsync()
		{
			var users = await this.Service.GetUsersAsync( CancellationToken.None );

			users.Users.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void GetUser()
		{
			var user = this.Service.GetUser( 6250887, CancellationToken.None );

			user.Should().NotBeNull();
		}

		[ Test ]
		public async Task GetUserAsync()
		{
			var user = await this.Service.GetUserAsync( 6250887, CancellationToken.None );

			user.Should().NotBeNull();
		}

		[ Test ]
		public void IsShopifyPlusAccount()
		{
			var result = this.Service.IsShopifyPlusAccount( CancellationToken.None );

			result.Should().Be( false );
		}

		[ Test ]
		public async Task IsShopifyPlusAccountAsync()
		{
			var result = await this.Service.IsShopifyPlusAccountAsync( CancellationToken.None );

			result.Should().Be( false );
		}
	}
}