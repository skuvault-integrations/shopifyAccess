using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Product
{
	[ DataContract ]
	public class ShopifyProducts
	{
		[ DataMember( Name = "products" ) ]
		public List< ShopifyProduct > Products { get; set; }

		public ShopifyProducts()
		{
			this.Products = new List< ShopifyProduct >();
		}
	}
}