using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.ProductVariant
{
	[ DataContract ]
	public class ProductVariant
	{
		[ DataMember( Name = "id" ) ]
		public string Id{ get; set; }

		[ DataMember( Name = "sku" ) ]
		public string Sku{ get; set; }
	}
}