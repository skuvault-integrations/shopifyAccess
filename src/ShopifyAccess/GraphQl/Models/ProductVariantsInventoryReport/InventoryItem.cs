using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.ProductVariantsInventoryReport
{
	[ DataContract ]
	internal class InventoryItem
	{
		[ DataMember( Name = "id" ) ]
		public string InventoryItemId{ get; set; }

		/// <summary>
		/// The fulfillment service that tracks the number of items in stock for the product variant.
		/// ProductVariant.InventoryManagement field is deprecated. Need to use this one instead
		/// </summary>
		[ DataMember( Name = "tracked" ) ]
		public bool Tracked{ get; set; }
	}
}