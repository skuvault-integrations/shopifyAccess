using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccess.Models.Product
{
	//TODO GUARD-3717 [Cleanup] Remove all [ Data* ] attributes since this will not be directly deserialized from GraphQL
	[ DataContract ]
	public class ShopifyProduct
	{
		[ DataMember( Name = "title" ) ]
		public string Title { get; set; }

		[ DataMember( Name = "variants" ) ]
		public List< ShopifyProductVariant > Variants { get; set; }

		[ DataMember( Name = "product_type" ) ]
		public string Type { get; set; }

		[ DataMember( Name = "images" ) ]
		public List< ShopifyProductImage > Images { get; set; }

		[ DataMember( Name = "vendor" ) ]
		public string Vendor { get; set; }

		[ DataMember( Name = "body_html" ) ]
		public string BodyHtml { get; set; }
		
		[ DataMember( Name = "updated_at" ) ]
		public DateTime UpdatedAt { get; set; }
	}
}