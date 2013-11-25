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
		public IList< ShopifyProductVariant > Variants { get; set; }
	}
}