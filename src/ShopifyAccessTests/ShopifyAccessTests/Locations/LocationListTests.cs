using System.Net;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Location;

namespace ShopifyAccessTests.Locations
{
	[ TestFixture ]
	public class LocationListTests : BaseTests
	{
		[ Test ]
		public void GetCorrectLocationList()
		{
			var locations = this.Service.GetLocations( CancellationToken.None );

			locations.Locations.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void LocationsNotLoaded_IncorrectToken()
		{
			var config = new ShopifyCommandConfig( this.Config.ShopName, "blabla" );
			var service = this.ShopifyFactory.CreateService( config );
			ShopifyLocations locations = null;
			try
			{
				locations = service.GetLocations( CancellationToken.None );
			}
			catch( WebException )
			{
				locations.Should().BeNull();
			}
		}

		[ Test ]
		public void LocationsNotLoaded_IncorrectShopName()
		{
			var config = new ShopifyCommandConfig( "blabla", this.Config.AccessToken );
			var service = this.ShopifyFactory.CreateService( config );
			ShopifyLocations locations = null;
			try
			{
				locations = service.GetLocations( CancellationToken.None );
			}
			catch( WebException )
			{
				locations.Should().BeNull();
			}
		}
	}
}