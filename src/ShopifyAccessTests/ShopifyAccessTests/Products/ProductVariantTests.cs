using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LINQtoCSV;
using Netco.Logging;
using Netco.Logging.NLogIntegration;
using NUnit.Framework;
using ShopifyAccess;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Product;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccessTests.Products
{
	[ TestFixture ]
	public class ProductVariantTests
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
		public void GetProducts()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			var products = service.GetProducts();

			products.Products.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetProductsAsync()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			var products = await service.GetProductsAsync();

			products.Products.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void ProductVariantQuantityUpdated()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );

			var variantToUpdate = new ShopifyProductVariantForUpdate { Id = 337095344, Quantity = 2 };
			service.UpdateProductVariants( new List< ShopifyProductVariantForUpdate > { variantToUpdate } );
		}

		[ Test ]
		public void GetAndUpdateProduct()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			var products = service.GetProducts().ToDictionary();

			var productsForUpdate = new List< ShopifyInventoryLevelForUpdate >();
			foreach( var product in products )
			{
				var firstInventoryLevel = product.Value.InventoryLevels.InventoryLevels.FirstOrDefault();
				if( firstInventoryLevel == null )
					continue;

				var productForUpdate = new ShopifyInventoryLevelForUpdate
				{
					InventoryItemId = firstInventoryLevel.InventoryItemId,
					LocationId = firstInventoryLevel.LocationId,
					Quantity = firstInventoryLevel.Available
				};

				if( product.Key.Equals( "T-BLA-S", StringComparison.InvariantCultureIgnoreCase ) )
				{
					productForUpdate.Quantity = 17;
					productsForUpdate.Add( productForUpdate );
				}
				
			}

			service.UpdateInventoryLevels( productsForUpdate );
			var updatedProducts = service.GetProducts().ToDictionary();
			
			products.Should().Equal( updatedProducts );
		}
	}
}