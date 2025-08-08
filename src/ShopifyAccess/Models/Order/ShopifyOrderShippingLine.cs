namespace ShopifyAccess.Models.Order
{
	public class ShopifyOrderShippingLine
	{
		public string Id{ get; set; }
		
		public string Title{ get; set; }
		
		public decimal Price{ get; set; }
		
		public string Code{ get; set; }
		
		public string Source{ get; set; }
	}
}