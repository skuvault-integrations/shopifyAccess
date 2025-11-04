using System;
using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Products
{
	internal class Product
	{
		[ DataMember( Name = "title" ) ]
		public string Title{ get; set; }

		[ DataMember( Name = "variants" ) ]
		//After GUARD-3946, variants below are null initially, when request products. They are filled in a separate call
		public Nodes< Products.ProductVariant > Variants{ get; set; }

		// [ JsonProperty( "id" ) ]
		public string Id{ get; set; }

		[ DataMember( Name = "vendor" ) ]
		public string Vendor{ get; set; }

		[ DataMember( Name = "media" ) ]
		public Nodes< Common.Media > Media{ get; set; }

		[ DataMember( Name = "productType" ) ]
		public string ProductType{ get; set; }

		[ DataMember( Name = "descriptionHtml" ) ]
		public string DescriptionHtml{ get; set; }

		[ DataMember( Name = "updatedAt" ) ]
		public DateTime? UpdatedAt{ get; set; }

		[ DataMember( Name = "createdAt" ) ]
		public DateTime? CreatedAt{ get; set; }
	}
}