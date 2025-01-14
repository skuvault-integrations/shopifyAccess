using System;
using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.Common;

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
		
		[ DataMember( Name = "image" ) ]
		public Image Image{ get; set; }
		
		[ DataMember( Name = "updatedAt" ) ]
		public DateTime? UpdatedAt{ get; set; }
	}
}