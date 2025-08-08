using System.Collections.Generic;

namespace ShopifyAccess.Models.Order
{
	public class ShopifyOrderItem
	{
		public string Id { get; set; }
		
		public string Sku { get; set; }

		public int Quantity { get; set; }

		public decimal Price { get; set; }

		public string Title{ get; set; }

		public bool ProductExists{ get; set; }

		public decimal TotalDiscount{ get; set; }

		public ShopifyPriceSet TotalDiscountSet{ get; set; }

		public IEnumerable< ShopifyTaxLine > TaxLines{ get; set; }
	}
}