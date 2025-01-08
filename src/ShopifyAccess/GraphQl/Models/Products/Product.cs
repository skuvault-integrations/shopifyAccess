using System;
using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.ProductVariantsInventory;

namespace ShopifyAccess.GraphQl.Models.Products
{
	internal class Product
	{
		//TODO GUARD-3717: Add properties
		[ DataMember( Name = "title" ) ]
		public string Title{ get; set; }
		
		[ DataMember( Name = "variants" ) ]
		//TODO GUARD-3717: Create a different Models/Products/ProductVariant class from the one in ProductVariantInventory, since need different fields:
		// Sku, UpdatedAt, Barcode, Title, Weight, WeightUnit, Price, ImageId
		public Nodes< ProductVariant > Variants{ get; set; }

		[ DataMember( Name = "vendor" ) ]
		public string Vendor{ get; set; }

		[ DataMember( Name = "productType" ) ]
		public string Type{ get; set; }

		[ DataMember( Name = "descriptionHtml" ) ]
		public string DescriptionHtml{ get; set; }

		[ DataMember( Name = "updatedAt" ) ]
		public DateTime UpdatedAt{ get; set; }
	}
}