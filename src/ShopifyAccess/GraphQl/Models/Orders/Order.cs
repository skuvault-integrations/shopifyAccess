using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using ShopifyAccess.Models.Order;

namespace ShopifyAccess.GraphQl.Models.Orders
{
	[ DataContract ]
	internal class Order
	{
		[ DataMember( Name = "id" ) ]
		public string Id{ get; set; }

		[ DataMember( Name = "totalPriceSet" ) ]
		public PriceSet Total{ get; set; }

		[ DataMember( Name = "createdAt" ) ]
		public DateTime CreatedAt{ get; set; }

		[ DataMember( Name = "lineItems" ) ]
		public Nodes< OrderItem > OrderItems{ get; set; }

		[ DataMember( Name = "number" ) ]
		public int Number{ get; set; }

		[ DataMember( Name = "billingAddress" ) ]
		public ShopifyBillingAddress BillingAddress{ get; set; }

		[ DataMember( Name = "shippingAddress" ) ]
		public ShopifyShippingAddress ShippingAddress{ get; set; }

		[ DataMember( Name = "closedAt" ) ]
		public DateTime? ClosedAt{ get; set; }

		[ DataMember( Name = "cancelledAt" ) ]
		public DateTime? CancelledAt{ get; set; }

		[ DataMember( Name = "displayFinancialStatus" ) ]
		//TODO PBL-9369 Handle a null financial_status the same way we do RawFulfillmentStatus -> FulfillmentStatus (Enum.TryParse< FulfillmentStatusEnum >)
		public ShopifyFinancialStatus FinancialStatus{ get; set; }

		[ DataMember( Name = "fulfillments" ) ]
		public IEnumerable< Fulfillment > Fulfillments{ get; set; }

		[ DataMember( Name = "displayFulfillmentStatus" ) ]
		private string RawFulfillmentStatus{ get; set; }

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

		[ DataMember( Name = "sourceName" ) ]
		public string RawSourceName{ get; set; }

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

		[ DataMember( Name = "name" ) ]
		public string Name{ get; set; }

		[ DataMember( Name = "shippingLines" ) ]
		public IList< ShippingLine > ShippingLines{ get; set; }

		[ DataMember( Name = "discountCodes" ) ]
		public string[] DiscountCodes{ get; set; }

		[ DataMember( Name = "taxLines" ) ]
		public IEnumerable< TaxLine > TaxLines{ get; set; }
	}
}