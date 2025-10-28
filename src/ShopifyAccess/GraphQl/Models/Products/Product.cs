using System;
using Newtonsoft.Json;

namespace ShopifyAccess.GraphQl.Models.Products
{
	internal class Product
	{
		[ JsonProperty( "title" ) ]
		public string Title{ get; set; }

		[ JsonProperty( "variants" ) ]
		public Nodes< ProductVariant > Variants{ get; set; }

		[ JsonProperty( "variantsCount" ) ]
		public VariantsCount TotalVariantsCount{ get; set; }

		[ JsonProperty( "vendor" ) ]
		public string Vendor{ get; set; }

		[ JsonProperty( "media" ) ]
		public Nodes< Common.Media > Media{ get; set; }

		[ JsonProperty( "productType" ) ]
		public string ProductType{ get; set; }

		[ JsonProperty( "descriptionHtml" ) ]
		public string DescriptionHtml{ get; set; }

		[ JsonProperty( "updatedAt" ) ]
		public DateTime? UpdatedAt{ get; set; }

		[ JsonProperty( "createdAt" ) ]
		public DateTime? CreatedAt{ get; set; }
	}

	internal class VariantsCount
	{
		[ JsonProperty( "count" ) ]
		public int? Count{ get; set; }
	}
}