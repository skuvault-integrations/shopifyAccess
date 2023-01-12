using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.ProductVariantsWithInventoryLevelsReport
{
	[ DataContract ]
	internal class ProductVariant
	{
		[ DataMember( Name = "id" ) ]
		public string ProductVariantId{ get; set; }

		[ DataMember( Name = "sku" ) ]
		public string Sku{ get; set; }

		[ DataMember( Name = "inventoryItem" ) ]
		public InventoryItem InventoryItem{ get; set; }

		public readonly List< InventoryLevel > InventoryLevels = new List< InventoryLevel >();
	}
}