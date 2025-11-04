using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ShopifyAccess.GraphQl.Models;
using ShopifyAccess.GraphQl.Models.Common;
using ShopifyAccess.GraphQl.Models.Products;
using ShopifyAccess.GraphQl.Models.Products.Extensions;

namespace ShopifyAccessTests.GraphQl.Models.Products.Extensions
{
	public class ProductExtensionsTests
	{
		private static readonly Randomizer _randomizer = new Randomizer();
		
		[ Test ]
		public void ToShopifyProduct_ShouldMapAllFieldsCorrectly_WhenAllTopLevelFieldsProvided()
		{
			var product = CreateProduct();
			
			var shopifyProduct = product.ToShopifyProduct();
			
			Assert.Multiple(() =>
			{
				Assert.That( shopifyProduct.Title, Is.EqualTo( product.Title ) );
				Assert.That( shopifyProduct.Vendor, Is.EqualTo( product.Vendor ) );
				Assert.That( shopifyProduct.Images.Single().Src, Is.EqualTo( product.Media.Items.Single().Preview.Image.Url ) );
				Assert.That( shopifyProduct.Variants.Single().Sku, Is.EqualTo( product.Variants.Items.Single().Sku ) );
				Assert.That( shopifyProduct.Type, Is.EqualTo( product.ProductType ) );
				Assert.That( shopifyProduct.BodyHtml, Is.EqualTo( product.DescriptionHtml ) );
				Assert.That( shopifyProduct.UpdatedAt, Is.EqualTo( product.UpdatedAt.Value ) );
			} );
		}

		[ Test ]
		public void ToShopifyProduct_ShouldReturnEmptyProductVariantsList_WhenVariantsListIsNull()
		{
			var product = CreateProduct();
			product.Variants = null; 
			
			var shopifyProduct = product.ToShopifyProduct();
			
			Assert.That( shopifyProduct.Variants, Is.Empty );
		}
		
		[ Test ]
		public void ToShopifyProduct_ShouldReturnEmptyImagesList_WhenProductMediaListIsNull()
		{
			var product = CreateProduct();
			product.Media = null;
			
			var shopifyProduct = product.ToShopifyProduct();
			
			Assert.That( shopifyProduct.Images, Is.Empty );
		}
		
		[ Test ]
		public void ToShopifyProduct_ShouldDefaultUpdatedAtDate_WhenDatePassedIsNull()
		{
			var product = CreateProduct();
			product.UpdatedAt = null;
			
			var shopifyProduct = product.ToShopifyProduct();
			
			Assert.That( shopifyProduct.UpdatedAt, Is.EqualTo( default( DateTime ) ) );
		}

		private static Product CreateProduct()
		{
			return new Product
			{
				Title = _randomizer.GetString(),
				Vendor = _randomizer.GetString(),
				Media = CreateMediaItems(),
				Variants = CreateVariants(),
				ProductType = _randomizer.GetString(),  
				DescriptionHtml = _randomizer.GetString(),
				UpdatedAt = DateTime.UtcNow
			};
		}

		private static Nodes< Media > CreateMediaItems() =>
			new Nodes< Media >
			{
				Items = new List< Media >
				{
					new Media
					{
						MediaContentType = "IMAGE",
						Preview = new MediaPreview
						{
							Image = new Image
							{
								Url = _randomizer.GetString()
							}
						}
					}
				}
			};
		
		private static Nodes< ProductVariant > CreateVariants() =>
			new Nodes< ProductVariant >
			{
				Items = new List< ProductVariant >
				{
					new ProductVariant
						{
							Sku = _randomizer.GetString(),
							Title = _randomizer.GetString(),
							Barcode = _randomizer.GetString(),
							Image = new Image
							{
								Url = _randomizer.GetString()
							},
							Price = _randomizer.NextDecimal(),
							InventoryItem = new InventoryItem
							{
								Measurement = new InventoryItemMeasurement
								{
									Weight = new Weight
									{
										Unit = "KILOGRAMS",
										Value = _randomizer.NextFloat()
									}
								}
							},
							UpdatedAt = DateTime.UtcNow
						}
				}
			};
	}
}