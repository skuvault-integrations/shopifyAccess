using System.Collections.Generic;
using System.Runtime.Serialization;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccess.Models.Product
{
	[ DataContract ]
	public class ShopifyProduct
	{
		[ DataMember( Name = "id" ) ]
		public long Id { get; set; }

		[ DataMember( Name = "variants" ) ]
		public List< ShopifyProductVariant > Variants { get; set; }

		[ DataMember( Name = "product_type" ) ]
		public string Type { get; set; }

		[ DataMember( Name = "images" ) ]
		public List< ShopifyProductImage > Images { get; set; }

		[ DataMember( Name = "vendor" ) ]
		public string Vendor { get; set; }

		[ DataMember( Name = "body_html" ) ]
		public string Description { get; set; }
		
		[ DataMember( Name = "tags" ) ]
		public string Note { get; set; }
	}
}