using System;
using ShopifyAccess.GraphQl.Models.Common;
using ShopifyAccess.Models.Product;

namespace ShopifyAccess.Models.ProductVariant
{
	public class ShopifyProductVariant
	{
		public long Id{ get; set; }

		public string Sku{ get; set; }

		public long InventoryItemId{ get; set; }

		public string Barcode{ get; set; }

		public string Title{ get; set; }

		public decimal Weight{ get; set; }

		public WeightUnit WeightUnit{ get; set; }

		public decimal Price{ get; set; }

		public DateTime UpdatedAt{ get; set; }

		public string ImageUrl{ get; set; }

		public ShopifyInventoryLevels InventoryLevels{ get; set; }
	}
}