using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ShopifyAccess.Models.Order;

namespace ShopifyAccess.GraphQl.Models.Orders
{
	internal class Order
	{
		[ DataMember( Name = "id" ) ]
		public long Id { get; set; }

		[ DataMember( Name = "totalPriceSet" ) ]
		public decimal Total { get; set; }

		[ DataMember( Name = "createdAt" ) ]
		public DateTime CreatedAt { get; set; }

		[ DataMember( Name = "lineItems" ) ]
		public Nodes< OrderItems > OrderItems { get; set; }

		[ DataMember( Name = "number" ) ]
		public int OrderNumber { get; set; }

		[ DataMember( Name = "billingAddress" ) ]
		public ShopifyBillingAddress BillingAddress { get; set; }

		[ DataMember( Name = "shippingAddress" ) ]
		public ShopifyShippingAddress ShippingAddress { get; set; }

		[ DataMember( Name = "closedAt" ) ]
		public DateTime? ClosedAt { get; set; }

		[ DataMember( Name = "cancelledAt" ) ]
		public DateTime? CancelledAt { get; set; }

		[ DataMember( Name = "displayFinancialStatus" ) ]
		public ShopifyFinancialStatus FinancialStatus { get; set; }

		[ DataMember( Name = "fulfillments" ) ]
		public IEnumerable< ShopifyFulfillment > Fulfillments { get; set; }
	}
}
