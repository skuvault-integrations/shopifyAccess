using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using ShopifyAccess.Models.Order.Discounts;

namespace ShopifyAccess.Models.Order
{
	[ DataContract ]
	public sealed class ShopifyOrder
	{
		[ DataMember( Name = "id" ) ]
		public long Id { get; set; }

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

		[ DataMember( Name = "closed_at" ) ]
		public DateTime? ClosedAt { get; set; }

		[ DataMember( Name = "cancelled_at" ) ]
		public DateTime? CancelledAt { get; set; }

		[ DataMember( Name = "financial_status" ) ]
		public ShopifyFinancialStatus FinancialStatus { get; set; }

		[ DataMember( Name = "fulfillments" ) ]
		public IEnumerable< ShopifyFulfillment > Fulfillments { get; set; }

		[ DataMember( Name = "fulfillment_status" ) ]
		private string RawFulfillmentStatus { get; set; }
		
		[ JsonIgnore ]
		public FulfillmentStatusEnum FulfillmentStatus
		{
			get
			{
				if ( Enum.TryParse< FulfillmentStatusEnum >( RawFulfillmentStatus, out var fulfillmentStatus ) )
				{
					return fulfillmentStatus;
				}
				else
				{
					return FulfillmentStatusEnum.Undefined;
				}
			}
			set
			{
				RawFulfillmentStatus = value.ToString();
			}
		}

		[ DataMember( Name = "source_name" ) ]
		public string RawSourceName { get; set; }

		[ JsonIgnore ]
		public ShopifySourceNameEnum SourceName
		{
			get
			{
				if ( Enum.TryParse< ShopifySourceNameEnum >( RawSourceName, out var sourceName ) )
				{
					return sourceName;
				}
				else
				{
					return ShopifySourceNameEnum.Undefined;
				}
			}
			set
			{
				RawSourceName = value.ToString();
			}
		}

		[ DataMember( Name = "location_id" ) ]
		public string LocationId { get; set; }

		[ DataMember( Name = "name" ) ]
		public string Name { get; set; }

		[ DataMember( Name = "shipping_lines" ) ]
		public IList<ShopifyOrderShippingLine> ShippingLines { get; set; }

		[ DataMember( Name = "discount_codes" ) ]
		public IEnumerable< ShopifyDiscountCode > DiscountCodes { get; set; }

		[ DataMember( Name = "tax_lines" ) ]
		public IEnumerable< ShopifyTaxLine > TaxLines { get; set; }

		[ DataMember( Name = "refunds" ) ]
		public IEnumerable< ShopifyOrderRefund > Refunds { get; set; }

		public bool IsShipped => this.ClosedAt.HasValue;

		public bool IsCancelled => this.CancelledAt.HasValue;
	}

	[ DataContract ]
	public sealed class ShopifyOrderRefund
	{
		[ DataMember( Name = "id" ) ]
		public long Id { get; set; }
		[ DataMember( Name = "order_id" ) ]
		public long OrderId { get; set; }
		[ DataMember( Name = "refund_line_items" ) ]
		public IEnumerable< ShopifyOrderRefundLineItem > RefundLineItems { get; set; }
	}

	[ DataContract ]
	public sealed class ShopifyOrderRefundLineItem
	{
		[ DataMember( Name = "id" ) ]
		public long Id { get; set; }
		[ DataMember( Name = "line_item_id" ) ]
		public long LineItemId { get; set; }
		[ DataMember( Name = "quantity" ) ]
		public int Quantity { get; set; }
		[ DataMember( Name = "restock_type" ) ]
		public string RestockType { get; set; }
	}

	public enum ShopifyFinancialStatus
	{
		// ReSharper disable InconsistentNaming
		Undefined,
		pending,
		authorized,
		partially_paid,
		paid,
		partially_refunded,
		refunded,
		voided
		// ReSharper restore InconsistentNaming
	}

	public enum FulfillmentStatusEnum
	{
		// ReSharper disable InconsistentNaming
		Undefined,
		fulfilled,
		partial
		// ReSharper restore InconsistentNaming
	}

	public enum ShopifySourceNameEnum
	{
		Undefined,
		web,
		pos,
		iphone,
		android
	}
}