using System.Collections.Generic;
using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.Products;
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
		//TODO GUARD-3939 Is this being returned as null from Walmart?
		public PriceSet Price { get; set; }
		
		[ DataMember( Name = "variant" ) ]
		//TODO GUARD-3939 Might be deceptive to use the full ProductVariant class here since we only get price
		//	Prob. clearer to create another type, with just "price"
		public ProductVariant Variant { get; set; }
		
		//TODO GUARD-3939 Or other price fields, see 1.a. here - https://linnworks.atlassian.net/browse/GUARD-3939?focusedCommentId=226232

		[ DataMember( Name = "title" ) ]
		public string Title { get; set; }

		[ DataMember( Name = "totalDiscountSet" ) ]
		public PriceSet TotalDiscount { get; set; }

		[ DataMember( Name = "taxLines" ) ]
		public IEnumerable< TaxLine > TaxLines { get; set; }
	}
}
