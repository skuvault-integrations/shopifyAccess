using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Orders
{
	internal class Refund
	{
		[ DataMember( Name = "id" ) ]
		public string Id{ get; set; }

		[ DataMember( Name = "refundLineItems" ) ]
		public Nodes< RefundLineItem > RefundLineItems{ get; set; }
	}

	internal class RefundLineItem
	{
		[ DataMember( Name = "id" ) ]
		public long Id{ get; set; }

		[ DataMember( Name = "lineItem" ) ]
		public RefundItem LineItem{ get; set; }
		
		[ DataMember( Name = "quantity" ) ]
		public int Quantity{ get; set; }

		[ DataMember( Name = "restock_type" ) ]
		public string RestockType{ get; set; }

	}

	internal class RefundItem
	{
		[ DataMember( Name = "id" ) ]
		public long Id{ get; set; }
	}
}
