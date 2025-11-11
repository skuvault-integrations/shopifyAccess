using System;
using System.IO;
using System.Linq;
using LINQtoCSV;
using Netco.Logging;
using NUnit.Framework;
using ShopifyAccess;
using ShopifyAccess.Models.Configuration.Command;

namespace ShopifyAccessTests
{
	public class BaseTests
	{
		protected static readonly ShopifyApiVersion ApiVersion = ShopifyApiVersion.V2025_07;
		protected readonly IShopifyFactory ShopifyFactory = new ShopifyFactory( ApiVersion );
		protected ShopifyClientCredentials _clientCredentials;
		protected IShopifyService Service;

		[ SetUp ]
		public void Init()
		{
			Directory.SetCurrentDirectory( TestContext.CurrentContext.TestDirectory );
			const string credentialsFilePath = @"..\..\..\Files\ShopifyCredentials.csv";
			NetcoLogger.LoggerFactory = new ConsoleLoggerFactory();

			var cc = new CsvContext();
			var testConfig = cc.Read< TestCommandConfig >( credentialsFilePath, new CsvFileDescription { FirstLineHasColumnNames = true } ).FirstOrDefault();
			if( testConfig != null )
			{
				this._clientCredentials = new ShopifyClientCredentials( testConfig.ShopName, testConfig.AccessToken );
				this.Service = this.ShopifyFactory.CreateService( this._clientCredentials );
			}

			// Some tests could fail with the "The free-quota limit on '20 ServiceStack.Text Types' has been reached" exception
			// Need to add ServiceStack license to this file
			const string serviceStackLicenseFilePath = @"..\..\Files\license.txt";
			if( File.Exists( serviceStackLicenseFilePath ) )
			{
				var licenseKey = File.ReadAllText( serviceStackLicenseFilePath );
				Environment.SetEnvironmentVariable( "SERVICESTACK_LICENSE", licenseKey );
			}
		}
	}
}
