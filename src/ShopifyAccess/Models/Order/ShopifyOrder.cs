using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order
{
	[ DataContract ]
	public class ShopifyOrder
	{
		[ DataMember( Name = "id" ) ]
		public string Id { get; set; }

		[ DataMember( Name = "total_price" ) ]
		public decimal Total { get; set; }

		[ DataMember( Name = "created_at" ) ]
		public DateTime CreatedAt { get; set; }

		[ DataMember( Name = "line_items" ) ]
		public IList< ShopifyOrderItem > OrderItems { get; set; }

		[ DataMember( Name = "order_number" ) ]
		public int OrderNumber { get; set; }

		[ DataMember( Name = "billing_address" ) ]
		public ShopifyBillingAddress BillingAddress { get; set; }

		[ DataMember( Name = "shipping_address" ) ]
		public ShopifyShippingAddress ShippingAddress { get; set; }

		[ DataMember( Name = "customer" ) ]
		public ShopifyCustomer Customer { get; set; }

		[ DataMember( Name = "closed_at" ) ]
		public DateTime? ClosedAt { get; set; }

		public bool IsShipped
		{
			get { return ClosedAt.HasValue; }
		}
	}

	public enum ShopifyFinantialStatus
	{
		// ReSharper disable InconsistentNaming
		Undefined,
		pending,
		partially_paid,
		paid,
		partially_refunded,
		refunded,
		voided
		// ReSharper restore InconsistentNaming
	}
}