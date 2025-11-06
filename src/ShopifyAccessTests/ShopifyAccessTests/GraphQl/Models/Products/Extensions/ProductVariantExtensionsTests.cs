using System;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ShopifyAccess.GraphQl.Models.Common;
using ShopifyAccess.GraphQl.Models.Products;
using ShopifyAccess.GraphQl.Models.Products.Extensions;

namespace ShopifyAccessTests.GraphQl.Models.Products.Extensions
{
	public class ProductVariantExtensionsTests
	{
		private static readonly Randomizer _randomizer = new Randomizer();
		
		[ Test ]
		public void ToToShopifyProductVariant_ShouldMapFieldsCorrectly_WhenTopLevelValuesProvided()
		{
			var productVariant = CreateProductVariant();
			
			var shopifyProductVariant = productVariant.ToShopifyProductVariant();
			
			Assert.Multiple(() => 
			{
				Assert.That( shopifyProductVariant.Sku, Is.EqualTo( productVariant.Sku ) );
				Assert.That( shopifyProductVariant.Title, Is.EqualTo( productVariant.Title ) );
				Assert.That( shopifyProductVariant.Barcode, Is.EqualTo( productVariant.Barcode ) );
				Assert.That( shopifyProductVariant.Weight, Is.EqualTo( ( decimal )productVariant.InventoryItem.Measurement.Weight.Value ) );
				Assert.That( shopifyProductVariant.WeightUnit, Is.EqualTo( productVariant.InventoryItem.Measurement.Weight.UnitStandardized ) );
				Assert.That( shopifyProductVariant.Price, Is.EqualTo( productVariant.Price.Value ) );
				Assert.That( shopifyProductVariant.ImageUrl, Is.EqualTo( productVariant.Image.Url ) );
				Assert.That( shopifyProductVariant.UpdatedAt, Is.EqualTo( productVariant.UpdatedAt ) );
			});
		}
		
		[ Test ]
		public void ToShopifyProductVariant_ShouldReturnNull_WhenProductVariantIsNull()
		{
			ProductVariant productVariant = null;
			
			var shopifyProductVariant = productVariant.ToShopifyProductVariant();
			
			Assert.That( shopifyProductVariant, Is.Null );
		}

		[ Test ]
		public void ToShopifyProductVariant_ShouldReturnZeroWeight_andDefaultUnits_WhenWeightIsNull()
		{
			var productVariant = CreateProductVariant();
			productVariant.InventoryItem.Measurement.Weight = null;
			
			var shopifyProductVariant = productVariant.ToShopifyProductVariant();
			
			Assert.Multiple(() => 
			{
				Assert.That( shopifyProductVariant.Weight, Is.EqualTo( default( decimal ) ) );
				Assert.That( shopifyProductVariant.WeightUnit, Is.EqualTo( WeightUnit.POUNDS ) );
			});
		}
		
		[ Test ]
		public void ToShopifyProductVariant_ShouldReturnZeroPrice_WhenPriceIsNull()
		{
			var productVariant = CreateProductVariant();
			productVariant.Price = null;
			
			var shopifyProductVariant = productVariant.ToShopifyProductVariant();
			
			Assert.That( shopifyProductVariant.Price, Is.EqualTo( default( decimal ) ) );
		}
		
		[ Test ]
		public void ToToShopifyProductVariant_ShouldReturnEmptyImageUrl_WhenImageIsNull()
		{
			var productVariant = CreateProductVariant();
			productVariant.Image = null;
			
			var shopifyProductVariant = productVariant.ToShopifyProductVariant();
			
			Assert.That( shopifyProductVariant.ImageUrl, Is.EqualTo( string.Empty ) );
		}

		private static ProductVariant CreateProductVariant() =>
			new ProductVariant
			{
				Sku = _randomizer.GetString(),
				Title = _randomizer.GetString(),
				Barcode = _randomizer.GetString(),
				InventoryItem = CreateInventoryItem(),
				Price = _randomizer.NextDecimal(),
				Image = new Image
				{
					Url = _randomizer.GetString()
				},
				UpdatedAt = DateTime.UtcNow
			};

		private static InventoryItem CreateInventoryItem() =>
			new InventoryItem
			{
				Measurement = new InventoryItemMeasurement
				{
					Weight = new Weight
					{
						Unit = _randomizer.NextEnum< WeightUnit >().ToString(),
						Value = _randomizer.NextFloat()
					}
				}
			};
	}
}