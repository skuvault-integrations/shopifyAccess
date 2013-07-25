using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order
{
	[ DataContract ]
	public class ShopifyOrderItem
	{
		[ DataMember( Name = "id" ) ]
		public string Id { get; set; }

		[ DataMember( Name = "id" ) ]
		public string Sku { get; set; }
	}
}