﻿using System.Linq;
using System.Net;
using FluentAssertions;
using LINQtoCSV;
using Netco.Logging;
using Netco.Logging.NLogIntegration;
using NUnit.Framework;
using ShopifyAccess;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Location;

namespace ShopifyAccessTests.Locations
{
	[ TestFixture ]
	public class LocationListTests
	{
		private readonly IShopifyFactory ShopifyFactory = new ShopifyFactory();
		private ShopifyCommandConfig Config;

		[ SetUp ]
		public void Init()
		{
			const string credentialsFilePath = @"..\..\Files\ShopifyCredentials.csv";
			NetcoLogger.LoggerFactory = new NLogLoggerFactory();

			var cc = new CsvContext();
			var testConfig = cc.Read< TestCommandConfig >( credentialsFilePath, new CsvFileDescription { FirstLineHasColumnNames = true } ).FirstOrDefault();

			if( testConfig != null )
				this.Config = new ShopifyCommandConfig( testConfig.ShopName, testConfig.AccessToken );
		}

		[ Test ]
		public void GetCorrectLocationList()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			ShopifyLocations locations = service.GetLocations();

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
				locations = service.GetLocations();
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
				locations = service.GetLocations();
			}
			catch( WebException )
			{
				locations.Should().BeNull();
			}
		}
	}
}