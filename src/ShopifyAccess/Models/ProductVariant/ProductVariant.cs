using System.Runtime.Serialization;

namespace ShopifyAccess.Models.ProductVariant
{
	[ DataContract( Name = "variant" ) ]
	public class ProductVariant
	{
		[ DataMember( Name = "id" ) ]
		public long Id { get; set; }

		[ DataMember( Name = "inventory_quantity" ) ]
		public int Quantity { get; set; }
	}
}