using System;
using Newtonsoft.Json;

namespace ShopifyAccess.GraphQl.Models.Products
{
	internal class Product
	{
		[ JsonProperty( "title" ) ]
		public string Title{ get; set; }

		[ JsonProperty( "id" ) ]
		public string Id{ get; set; }

		[ JsonProperty( "variants" ) ]
		//TODO GUARD-3946 11.4 BOD Definitely DO NOT get variants in GetProducts query at all, in which case this will be removed in the cleanup
		//	So create a legacy version of this class and restore it. Then in this non-legacy one, just need the productId but no First250Variants or TotalVariantsCount
		public Nodes< ProductVariant > First250Variants{ get; set; }

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