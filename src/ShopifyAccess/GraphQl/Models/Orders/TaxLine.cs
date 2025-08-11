using System.Runtime.Serialization;
using ShopifyAccess.Models.Order;

namespace ShopifyAccess.GraphQl.Models.Orders
{
	internal class TaxLine
	{
		[ DataMember( Name = "title" ) ]
		public string Title{ get; set; }

		[ DataMember( Name = "priceSet" ) ]
		public ShopifyPriceSet PriceSet{ get; set; }
	}
}
