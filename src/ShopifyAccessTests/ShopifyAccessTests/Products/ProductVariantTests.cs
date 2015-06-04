using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LINQtoCSV;
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
			const string credentialsFilePath = @"..\..\Files\ShopifyCredentials.csv";

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

			var variantToUpdate = new ShopifyProductVariant { Id = 337095344, Quantity = 2, InventoryManagement = InventoryManagement.Shopify };
			service.UpdateProductVariants( new List< ShopifyProductVariant > { variantToUpdate } );
		}

		[ Test ]
		public void GetAndUpdateProduct()
		{
			var service = this.ShopifyFactory.CreateService( this.Config );
			var products = service.GetProducts().ToDictionary();
			ShopifyProductVariant variant;
			if( products.TryGetValue( "dkTestSku", out variant ) )
				variant.Quantity = 7;

			service.UpdateProductVariants( products.Values );
			var updatedProducts = service.GetProducts().ToDictionary();
			
			products.Should().Equal( updatedProducts );
		}
	}
}