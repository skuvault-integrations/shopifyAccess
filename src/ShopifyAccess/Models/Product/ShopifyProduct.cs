using System;
using System.Collections.Generic;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccess.Models.Product
{
	public class ShopifyProduct
	{
		public string Title { get; set; }

		public List< ShopifyProductVariant > Variants { get; set; }

		public string Type { get; set; }

		public List< ShopifyProductImage > Images { get; set; }

		public string Vendor { get; set; }

		public string BodyHtml { get; set; }
		
		public DateTime UpdatedAt { get; set; }
	}
}