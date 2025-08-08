using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ShopifyAccess.Models.Order.Discounts;

namespace ShopifyAccess.Models.Order
{
	public sealed class ShopifyOrder
	{
		public long Id { get; set; }
		
		public decimal Total { get; set; }
		
		public DateTime CreatedAt { get; set; }
		
		public IList< ShopifyOrderItem > OrderItems { get; set; }
		
		public int OrderNumber { get; set; }
		
		public ShopifyBillingAddress BillingAddress { get; set; }
		
		public ShopifyShippingAddress ShippingAddress { get; set; }
		
		public DateTime? ClosedAt { get; set; }
		
		public DateTime? CancelledAt { get; set; }

		public ShopifyFinancialStatus FinancialStatus { get; set; }

		public IEnumerable< ShopifyFulfillment > Fulfillments { get; set; }

		private string RawFulfillmentStatus { get; set; }

		[ JsonIgnore ]
		public FulfillmentStatusEnum FulfillmentStatus
		{
			get
			{
				if( Enum.TryParse< FulfillmentStatusEnum >( this.RawFulfillmentStatus, true, out var fulfillmentStatus ) )
					return fulfillmentStatus;
				return FulfillmentStatusEnum.Undefined;
			}
			set => this.RawFulfillmentStatus = value.ToString();
		}

		public string RawSourceName { get; set; }

		[ JsonIgnore ]
		public ShopifySourceNameEnum SourceName
		{
			get
			{
				if( Enum.TryParse< ShopifySourceNameEnum >( this.RawSourceName, true, out var sourceName ) )
					return sourceName;
				return ShopifySourceNameEnum.Undefined;
			}
			set => this.RawSourceName = value.ToString();
		}

		public string LocationId { get; set; }

		public string Name { get; set; }

		public IList<ShopifyOrderShippingLine> ShippingLines { get; set; }

		public IEnumerable< ShopifyDiscountCode > DiscountCodes { get; set; }

		public IEnumerable< ShopifyTaxLine > TaxLines { get; set; }

		public IEnumerable< ShopifyOrderRefund > Refunds { get; set; }

		public bool IsShipped => this.ClosedAt.HasValue;

		public bool IsCancelled => this.CancelledAt.HasValue;
	}
	
	public sealed class ShopifyOrderRefund
	{
		public long Id { get; set; }
		public long OrderId { get; set; }
		public IEnumerable< ShopifyOrderRefundLineItem > RefundLineItems { get; set; }
	}
	
	public sealed class ShopifyOrderRefundLineItem
	{
		public long Id { get; set; }
		public long LineItemId { get; set; }
		public int Quantity { get; set; }
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