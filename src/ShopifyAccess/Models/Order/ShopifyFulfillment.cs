using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.Common;

namespace ShopifyAccess.Models.Order
{
	[ DataContract ]
	public class ShopifyFulfillment
	{
		private static readonly Dictionary< string, ShopifyOrderFulfillmentStatusEnum > _statusesMapping = new Dictionary< string, ShopifyOrderFulfillmentStatusEnum >()
		{
			{ "pending", ShopifyOrderFulfillmentStatusEnum.Pending },
			{ "open", ShopifyOrderFulfillmentStatusEnum.Open },
			{ "success", ShopifyOrderFulfillmentStatusEnum.Success },
			{ "cancelled", ShopifyOrderFulfillmentStatusEnum.Cancelled },
			{ "error", ShopifyOrderFulfillmentStatusEnum.Error },
			{ "failure", ShopifyOrderFulfillmentStatusEnum.Failure },
		};

		public ShopifyFulfillment()
		{
		}

		public ShopifyFulfillment( string status )
		{
			this.StatusValue = status;
		}

		[ DataMember( Name = "created_at" ) ]
		public DateTime CreatedAt{ get; set; }

		[ DataMember( Name = "id" ) ]
		public long Id{ get; set; }

		[ DataMember( Name = "order_id" ) ]
		public long OrderId{ get; set; }

		[ DataMember( Name = "status" ) ]
		private string StatusValue{ get; set; }

		public ShopifyOrderFulfillmentStatusEnum Status
		{
			get
			{
				var status = ShopifyOrderFulfillmentStatusEnum.Undefined;
				_statusesMapping.TryGetValue( this.StatusValue, out status );
				return status;
			}
		}

		[ DataMember( Name = "tracking_company" ) ]
		public string TrackingCompany{ get; set; }

		[ DataMember( Name = "tracking_number" ) ]
		public string TrackingNumber{ get; set; }

		[ DataMember( Name = "updated_at" ) ]
		public DateTime UpdatedAt{ get; set; }

		[ DataMember( Name = "tracking_url" ) ]
		public string TrackingUrl{ get; set; }

		[ DataMember( Name = "line_items" ) ]
		public IEnumerable< ShopifyOrderFulfillmentItem > Items{ get; set; }
	}

	[ DataContract ]
	public class ShopifyOrderFulfillmentItem
	{
		[ DataMember( Name = "id" ) ]
		public long Id{ get; set; }

		[ DataMember( Name = "sku" ) ]
		public string Sku{ get; set; }

		[ DataMember( Name = "quantity" ) ]
		public int Quantity{ get; set; }

		// TODO GUARD-3910: Remove this field, as we’ll be using Weight instead.
		[ DataMember( Name = "grams" ) ]
		public int Grams{ get; set; }

		[ DataMember( Name = "price" ) ]
		public decimal Price{ get; set; }

		public Weight Weight{ get; set; }
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