using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.Products;

namespace ShopifyAccess.GraphQl.Models.Orders
{
	internal class Fulfillment
	{
		[ DataMember( Name = "id" ) ]
		public string Id{ get; set; }

		[ DataMember( Name = "createdAt" ) ]
		public DateTime CreatedAt{ get; set; }

		[ DataMember( Name = "updatedAt" ) ]
		public DateTime UpdatedAt{ get; set; }

		[ DataMember( Name = "status" ) ]
		public string Status{ get; set; }

		[ DataMember( Name = "location" ) ]
		public FulfillmentLocation Location{ get; set; }

		[ DataMember( Name = "trackingInfo" ) ]
		public IEnumerable< TrackingInfo > TrackingInfo{ get; set; }

		[ DataMember( Name = "fulfillmentLineItems" ) ]
		public Nodes< FulfillmentLineItem > FulfillmentLineItems{ get; set; }
	}

	[ DataContract ]
	internal class TrackingInfo
	{
		[ DataMember( Name = "company" ) ]
		public string TrackingCompany{ get; set; }

		[ DataMember( Name = "number" ) ]
		public string TrackingNumber{ get; set; }

		[ DataMember( Name = "url" ) ]
		public string TrackingUrl{ get; set; }
	}

	internal class FulfillmentLocation
	{
		[ DataMember( Name = "id" ) ]
		public string Id{ get; set; }
	}

	internal class FulfillmentLineItem
	{
		[ DataMember( Name = "lineItem" ) ]
		public LineItemDetail LineItem{ get; set; }
	}

	internal class LineItemDetail
	{
		[ DataMember( Name = "id" ) ]
		public string Id{ get; set; }

		[ DataMember( Name = "sku" ) ]
		public string Sku{ get; set; }

		[ DataMember( Name = "quantity" ) ]
		public int Quantity{ get; set; }

		//TODO GUARD-3946 Problem: this is not requested by query GetOrdersPaginated > fulfillments > fulfillmentLineItems > nodes > lineItem
		//	So removed, to avoid unnecessary C# dependence. Confirm
		//[ DataMember( Name = "product" ) ]
		//public Product Product{ get; set; }

		[ DataMember( Name = "originalUnitPriceSet" ) ]
		public PriceSet OriginalUnitPriceSet{ get; set; }

		[ DataMember( Name = "variant" ) ]
		public ProductVariant Variant{ get; set; }
	}
}