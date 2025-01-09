using System;
using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Products
{
	internal class Product
	{
		//TODO GUARD-3717: Finish adding properties
		[ DataMember( Name = "title" ) ]
		public string Title{ get; set; }
		
		[ DataMember( Name = "variants" ) ]
		public Nodes< Products.ProductVariant > Variants{ get; set; }

		[ DataMember( Name = "vendor" ) ]
		public string Vendor{ get; set; }
		
		//TODO GUARD-3717: Images if needed
		// media(first: 250)
		// {
		// 	nodes
		// 	{
		// 		id
		// 			mediaContentType
		// 	}
		// }

		[ DataMember( Name = "productType" ) ]
		public string Type{ get; set; }

		[ DataMember( Name = "descriptionHtml" ) ]
		public string DescriptionHtml{ get; set; }

		[ DataMember( Name = "updatedAt" ) ]
		public DateTime? UpdatedAt{ get; set; }
	}
}