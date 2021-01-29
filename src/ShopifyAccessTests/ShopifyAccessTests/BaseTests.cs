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
		protected readonly IShopifyFactory ShopifyFactory = new ShopifyFactory();
		protected ShopifyCommandConfig Config;
		protected IShopifyService Service;

		[ SetUp ]
		public void Init()
		{
			Directory.SetCurrentDirectory( TestContext.CurrentContext.TestDirectory );
			const string credentialsFilePath = @"..\..\Files\ShopifyCredentials.csv";
			NetcoLogger.LoggerFactory = new ConsoleLoggerFactory();

			var cc = new CsvContext();
			var testConfig = cc.Read< TestCommandConfig >( credentialsFilePath, new CsvFileDescription { FirstLineHasColumnNames = true } ).FirstOrDefault();
			if( testConfig != null )
			{
				this.Config = new ShopifyCommandConfig( testConfig.ShopName, testConfig.AccessToken );
				this.Service = this.ShopifyFactory.CreateService( this.Config );
			}
		}
	}
}
