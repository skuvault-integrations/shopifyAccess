using System.Collections.Generic;
using System.Runtime.Serialization;
using ShopifyAccess.Models.Order;

namespace ShopifyAccess.GraphQl.Models.Orders
{
	internal class OrderItem
	{
		[ DataMember( Name = "id" ) ]
		public string Id { get; set; }

		[ DataMember( Name = "sku" ) ]
		public string Sku { get; set; }

		[ DataMember( Name = "quantity" ) ]
		public int Quantity { get; set; }

		[ DataMember( Name = "originalUnitPriceSet" ) ]
		public PriceSet Price { get; set; }

		[ DataMember( Name = "title" ) ]
		public string Title { get; set; }

		[ DataMember( Name = "totalDiscountSet" ) ]
		public PriceSet TotalDiscount { get; set; }

		[ DataMember( Name = "taxLines" ) ]
		public IEnumerable< TaxLine > TaxLines { get; set; }
	}
}
