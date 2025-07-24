using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Orders
{
	public class OrderItems
	{
		[ DataMember( Name = "id" ) ]
		public string Id { get; set; }

		[ DataMember( Name = "sku" ) ]
		public string Sku { get; set; }

		[ DataMember( Name = "quantity" ) ]
		public int Quantity { get; set; }

		[ DataMember( Name = "originalUnitPriceSet" ) ]
		public decimal Price { get; set; }

		[ DataMember( Name = "title" ) ]
		public string Title{ get; set; }

		[ DataMember( Name = "total_discount" ) ]
		public decimal TotalDiscount{ get; set; }

		// [ DataMember( Name = "totalDiscountSet" ) ]
		// public ShopifyPriceSet TotalDiscountSet{ get; set; }
		//
		// [ DataMember( Name = "tax_lines" ) ]
		// public IEnumerable< ShopifyTaxLine > TaxLines{ get; set; }
	}
}
