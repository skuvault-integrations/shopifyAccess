using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.ProductVariant
{
	[ DataContract ]
	public class ProductVariantsData
	{
		[ DataMember( Name = "productVariants" ) ]
		public ProductVariants ProductVariants{ get; set; }
	}
}