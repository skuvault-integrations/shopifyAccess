using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order
{
	[ DataContract ]
	public class ShopifyTaxLine
	{
		[ DataMember( Name = "title" ) ]
		public string Title{ get; set; }

		[ DataMember( Name = "price_set" ) ]
		public ShopifyPriceSet PriceSet{ get; set; }
	}
}
