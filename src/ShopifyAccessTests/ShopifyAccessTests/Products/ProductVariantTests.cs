using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LINQtoCSV;
using Netco.Logging;
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
		private IShopifyService _service;

		[ SetUp ]
		public void Init()
		{
			Directory.SetCurrentDirectory( TestContext.CurrentContext.TestDirectory );
			const string credentialsFilePath = @"..\..\Files\ShopifyCredentials.csv";
			NetcoLogger.LoggerFactory = new ConsoleLoggerFactory();

			var cc = new CsvContext();
			var testConfig = cc.Read< TestCommandConfig >( credentialsFilePath, new CsvFileDescription { FirstLineHasColumnNames = true } ).FirstOrDefault();

			if( testConfig != null )
				this._service = this.ShopifyFactory.CreateService( new ShopifyCommandConfig( testConfig.ShopName, testConfig.AccessToken ) );
		}

		[ Test ]
		public void GetProducts()
		{
			var products = this._service.GetProducts( CancellationToken.None );

			products.Products.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetProductsAsync()
		{
			var products = await this._service.GetProductsAsync( CancellationToken.None );

			products.Products.Count.Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public async Task GetProductsCreatedAfterAsync()
		{
			var productsStartUtc = new DateTime( 1800, 1, 1 );

			var products = await this._service.GetProductsCreatedAfterAsync( productsStartUtc, CancellationToken.None );

			products.Products.Count.Should().BeGreaterThan( 250 );
		}

		[ Test ]
		public async Task GetProductsCreatedAfterAsync_GetsVariationsWithUntrackedQuantity()
		{
			var products = await this._service.GetProductsCreatedAfterAsync( DateTime.MinValue, CancellationToken.None );

			products.Products.Any( p => p.Variants.Any( v => v.InventoryManagement == InventoryManagement.Blank ) );
		}

		[ Test ]
		public async Task GetProductsThroughLocationsAsync()
		{
			var products = await this._service.GetProductsInventoryAsync( CancellationToken.None );

			products.Products.Should().NotBeNullOrEmpty();
		}

		[ Test ]
		public void ProductVariantQuantityUpdated()
		{
			var variantToUpdate = new ShopifyProductVariantForUpdate { Id = 337095344, Quantity = 2 };
			this._service.UpdateProductVariants( new List< ShopifyProductVariantForUpdate > { variantToUpdate }, CancellationToken.None );
		}

		[ Test ]
		public void GetAndUpdateProductQuantity()
		{
			const string sku = "testSku1";
			var products = this._service.GetProducts( CancellationToken.None ).ToDictionary();
			const int quantity = 17;

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

				if( product.Key.Equals( sku, StringComparison.InvariantCultureIgnoreCase ) )
				{
					productForUpdate.Quantity = quantity;
					productsForUpdate.Add( productForUpdate );
					break;
				}
			}

			_service.UpdateInventoryLevels( productsForUpdate, CancellationToken.None );
			var updatedProducts = this._service.GetProducts( CancellationToken.None ).ToDictionary();
			
			var updatedProduct = updatedProducts.FirstOrDefault( p => p.Key.Equals( sku, StringComparison.InvariantCultureIgnoreCase ) );
			updatedProduct.Value.InventoryLevels.InventoryLevels[ 0 ].Available.Should().Be( quantity );
		}

		[ Test ]
		public async Task GetAndUpdateProductAsync()
		{
			const string sku = "testsku1";
			var inventoryItem = await this.GetFirstInventoryItem( sku );
			var initialQuantity = inventoryItem.Available;
			const int quantity = 39;

			await this._service.UpdateInventoryLevelsAsync( CreateInventoryLevelForUpdate( inventoryItem, quantity ), CancellationToken.None );
			var product = ( await this._service.GetProductVariantsInventoryBySkusAsync( new List< string > { sku }, CancellationToken.None ) ).First();
			var newQuantity = product.InventoryLevels.InventoryLevels.First().Available;
			await this._service.UpdateInventoryLevelsAsync( CreateInventoryLevelForUpdate( inventoryItem, initialQuantity ), CancellationToken.None );

			newQuantity.Should().Be( quantity );
		}

		private async Task< ShopifyInventoryLevel > GetFirstInventoryItem( string sku )
		{
			var product = ( await this._service.GetProductVariantsInventoryBySkusAsync( new List< string > { sku }, CancellationToken.None ) ).First();
			return product.InventoryLevels.InventoryLevels.First();
		}

		private static IEnumerable< ShopifyInventoryLevelForUpdate > CreateInventoryLevelForUpdate( ShopifyInventoryLevel inventory, int setQuantity )
		{
			IEnumerable< ShopifyInventoryLevelForUpdate > inventoryLevels = new List< ShopifyInventoryLevelForUpdate >
			{
				new ShopifyInventoryLevelForUpdate
				{
					InventoryItemId = inventory.InventoryItemId,
					Quantity = setQuantity,
					LocationId = inventory.LocationId
				}
			};
			return inventoryLevels;
		}
	}
}