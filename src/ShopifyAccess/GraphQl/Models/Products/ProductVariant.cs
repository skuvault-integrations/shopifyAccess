using System;
using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Products
{
	internal class ProductVariant
	{
		[ DataMember( Name = "sku" ) ]
		public string Sku{ get; set; }
		
		[ DataMember( Name = "title" ) ]
		public string Title{ get; set; }
		
		[ DataMember( Name = "barcode" ) ]
		public string Barcode{ get; set; }
		
		[ DataMember( Name = "inventoryItem" ) ]
		public Products.InventoryItem InventoryItem{ get; set; }
		
		[ DataMember( Name = "price" ) ]
		public decimal? Price{ get; set; }
		
		//TODO 3717: image
		// image
		// {
		// 	id
		// 		url
		// }
		
		[ DataMember( Name = "updatedAt" ) ]
		public DateTime? UpdatedAt{ get; set; }
	}
}