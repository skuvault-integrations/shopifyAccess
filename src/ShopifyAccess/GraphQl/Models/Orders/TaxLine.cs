using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Orders
{
	internal class TaxLine
	{
		[ DataMember( Name = "title" ) ]
		public string Title{ get; set; }

		[ DataMember( Name = "priceSet" ) ]
		public PriceSet PriceSet{ get; set; }
	}
}