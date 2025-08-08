using System;
using System.Collections.Generic;

namespace ShopifyAccess.Models.Order
{
	public class ShopifyFulfillment
	{
		private static Dictionary< string, ShopifyOrderFulfillmentStatusEnum > _statusesMapping = new Dictionary< string, ShopifyOrderFulfillmentStatusEnum >()
		{
			{ "pending", ShopifyOrderFulfillmentStatusEnum.Pending },
			{ "open", ShopifyOrderFulfillmentStatusEnum.Open },
			{ "success", ShopifyOrderFulfillmentStatusEnum.Success },
			{ "cancelled", ShopifyOrderFulfillmentStatusEnum.Cancelled },
			{ "error", ShopifyOrderFulfillmentStatusEnum.Error },
			{ "failure", ShopifyOrderFulfillmentStatusEnum.Failure },
		};
		
		public ShopifyFulfillment(){}

		public ShopifyFulfillment( string status )
		{
			this.StatusValue = status;
		}

		public DateTime CreatedAt { get; set; }

		public long Id { get; set; }

		public long OrderId { get; set; }

		private string StatusValue { get; set; }
		
		public ShopifyOrderFulfillmentStatusEnum Status
		{
			get
			{
				var status = ShopifyOrderFulfillmentStatusEnum.Undefined;
				_statusesMapping.TryGetValue( this.StatusValue, out status );
				return status;
			}
		}
		
		public string TrackingCompany { get; set; }
		
		public string TrackingNumber { get; set; }
		
		public DateTime UpdatedAt { get; set; }
		
		public string TrackingUrl { get; set; }
		
		public IEnumerable< ShopifyOrderFulfillmentItem > Items { get; set; }
	}
	
	public class ShopifyOrderFulfillmentItem
	{
		public long Id { get; set; }
		
		public string Sku { get; set; }
		
		public int Quantity { get; set; }
		
		public int Grams { get; set; }
		
		public decimal Price { get; set; }
	}

	public enum ShopifyOrderFulfillmentStatusEnum
	{
		Undefined,
		Pending,
		Open,
		Success,
		Cancelled,
		Error,
		Failure
	}

	public enum ShopifyOrderStatus
	{
		Undefined,
		open,
		closed,
		cancelled,
		any
	}

	public enum ShopifyOrderFinancialStatus
	{
		Undefined,
		authorized,
		pending,
		paid,
		partially_paid,
		abandoned,
		refunded,
		voided,
		any
	}
}