using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Product
{
	[ DataContract ]
	public class ProductsCount
	{
		[ DataMember( Name = "count" ) ]
		public int Count { get; set; }
	}
}