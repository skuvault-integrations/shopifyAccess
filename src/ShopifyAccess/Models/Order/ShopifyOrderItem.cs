using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order
{
	[ DataContract ]
	public class ShopifyOrderItem
	{
		[ DataMember( Name = "id" ) ]
		public string Id { get; set; }
		
		[ DataMember( Name = "sku" ) ]
		public string Sku { get; set; }

		[ DataMember( Name = "quantity" ) ]
		public int Quantity { get; set; }
		
		[ DataMember( Name = "price" ) ]
		public decimal Price { get; set; }

		[ DataMember( Name = "title" ) ]
		public string Title{ get; set; }

		[ DataMember( Name = "product_exists" ) ]
		public bool ProductExists{ get; set; }

		[ DataMember( Name = "total_discount" ) ]
		public decimal TotalDiscount{ get; set; }

		[ DataMember( Name = "total_discount_set" ) ]
		public ShopifyPriceSet TotalDiscountSet{ get; set; }

		[ DataMember( Name = "tax_lines" ) ]
		public IEnumerable< ShopifyTaxLine > TaxLines{ get; set; }
	}
}