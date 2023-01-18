using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Helpers;
using ShopifyAccess.Models.Product;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccess.GraphQl.Models.ProductVariantsInventoryReport
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

		public ShopifyProductVariant ToShopifyProductVariant()
		{
			var inventoryItemId = GraphQlIdParser.InventoryItem.GetId( this.InventoryItem?.InventoryItemId );
			return new ShopifyProductVariant()
			{
				Id = GraphQlIdParser.ProductVariant.GetId( this.ProductVariantId ),
				Sku = this.Sku,
				InventoryItemId = inventoryItemId,
				InventoryLevels = GetShopifyInventoryLevels( this.InventoryLevels, inventoryItemId )
			};
		}

		private static ShopifyInventoryLevels GetShopifyInventoryLevels( IEnumerable< InventoryLevel > inventories, long inventoryItemId )
		{
			var result = new ShopifyInventoryLevels();
			result.InventoryLevels.AddRange( inventories.Select( inv => new ShopifyInventoryLevel()
			{
				InventoryItemId = inventoryItemId,
				Available = inv.Available,
				LocationId = GraphQlIdParser.Location.GetId( inv.Location?.LocationId )
			} ) );
			return result;
		}
	}
}