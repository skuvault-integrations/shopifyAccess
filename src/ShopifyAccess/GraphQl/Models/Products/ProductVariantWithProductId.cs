using Newtonsoft.Json;

namespace ShopifyAccess.GraphQl.Models.Products
{
	internal class ProductVariantWithProductId :  ProductVariant
	{
		[ JsonProperty( "product" ) ]
		public VariantParentProduct Product{ get; set; }
	}
	
	internal class VariantParentProduct
	{
		[ JsonProperty( "id" ) ]
		public string Id{ get; set; }
	}
}