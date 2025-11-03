using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.Exceptions;
using ShopifyAccess.Models.Configuration.Command;

namespace ShopifyAccessTests.Locations
{
	[ Explicit ]
	[ TestFixture ]
	public class LocationListTests : BaseTests
	{
		[ Explicit ]
		[ Test ]
		public void GetLocations_ReturnsLocationList()
		{
			// Act
			var locations = this.Service.GetLocations( CancellationToken.None );

			// Assert
			locations.Locations.Should().HaveCountGreaterThan( 0 );
		}

		[ Test ]
		public void GetLocations_ThrowsShopifyUnauthorizedException_WhenIncorrectToken()
		{
			// Arrange
			var clientCredentials = new ShopifyClientCredentials( this._clientCredentials.ShopName, "blabla" );
			var service = this.ShopifyFactory.CreateService( clientCredentials );

			// Act, Assert
			service.Invoking( s => s.GetLocations( CancellationToken.None ) ).Should().Throw< ShopifyUnauthorizedException >();
		}

		[ Test ]
		public void GetLocations_ThrowsShopifyUnauthorizedException_WhenIncorrectShopName()
		{
			// Arrange
			var clientCredentials = new ShopifyClientCredentials( "blabla", this._clientCredentials.AccessToken );
			var service = this.ShopifyFactory.CreateService( clientCredentials );

			// Act, Assert
			service.Invoking( s => s.GetLocations( CancellationToken.None ) ).Should().Throw< ShopifyUnauthorizedException >();
		}

		[ Test ]
		[ Explicit ]
		public async Task GetActiveLocationsAsync_ReturnsOnlyActiveLocations()
		{
			var result = await this.Service.GetActiveLocationsAsync( CancellationToken.None ).ConfigureAwait( false );

			Assert.That( result.Locations.All( x => x.IsActive ), Is.True );
		}
	}
}