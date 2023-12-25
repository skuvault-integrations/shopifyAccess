using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.ProductVariantsInventory
{
	/// <summary>
	/// Represents an available quantity of an inventory item at a specific location
	/// </summary>
	[ DataContract ]
	internal class InventoryQuantity
	{
		/// <summary>
		/// The quantity for the quantity name
		/// </summary>
		[ DataMember( Name = "quantity") ]
		public int Quantity{ get; set; }
	}
}