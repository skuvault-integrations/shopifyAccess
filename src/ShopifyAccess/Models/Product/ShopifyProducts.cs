using System.Collections.Generic;

namespace ShopifyAccess.Models.Product
{
	public class ShopifyProducts
	{
		public List< ShopifyProduct > Products { get; set; }

		public ShopifyProducts()
		{
			this.Products = new List< ShopifyProduct >();
		}
	}
}