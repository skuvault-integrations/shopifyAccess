using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ShopifyAccess.Models.Product;

namespace ShopifyAccess.Models.ProductVariant
{
	[ DataContract ]
	public class ShopifyProductVariant: IEquatable< ShopifyProductVariant >
	{
		[ DataMember( Name = "id" ) ]
		public long Id{ get; set; }

		/// <summary>
		/// This field is obsolete. DON'T USE IT
		/// </summary>
		[ DataMember( Name = "inventory_quantity" ) ]
		public int Quantity{ get; set; }

		/// <summary>
		/// This field is obsolete. DON'T USE IT
		/// </summary>
		[DataMember( Name = "old_inventory_quantity" ) ]
		public int OldQuantity{ get; set; }

		[ DataMember( Name = "inventory_management" ) ]
		public InventoryManagement InventoryManagement{ get; set; }

		[ DataMember( Name = "sku" ) ]
		public string Sku{ get; set; }

		[ DataMember( Name = "inventory_item_id" ) ]
		public long InventoryItemId{ get; set; }

		[ DataMember( Name = "barcode" ) ]
		public string Barcode { get; set; }
		
		[ DataMember( Name = "title" ) ]
		public string Title { get; set; }

		[ DataMember( Name = "weight" ) ]
		public decimal Weight;

		[ DataMember( Name = "weight_unit" ) ]
		protected string WeightUnit { get; set; }
		public string WeightUnitStandardized => this.WeightUnit?.Replace("kg", "kgs").Replace("lb", "lbs") ?? "lbs";

		[ DataMember( Name = "price" ) ]
		public decimal Price { get; set; }

		[ DataMember( Name = "updated_at" ) ]
		public DateTime UpdatedAt { get; set; }

		[ DataMember( Name = "image_id" ) ]
		public long ImageId { get; set; }

		public ShopifyInventoryLevels InventoryLevels{ get; set; }

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = this.Id.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ this.Quantity.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ this.InventoryManagement.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ this.Sku.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ this.Barcode.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ this.Title.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ this.Weight.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ this.WeightUnit.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ this.Price.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ this.UpdatedAt.GetHashCode();

				return hashCode;
			}
		}

		public bool Equals( ShopifyProductVariant other )
		{
			return this.Id.Equals( other.Id ) &&
			       this.Quantity.Equals( other.Quantity ) &&
			       this.InventoryManagement.Equals( other.InventoryManagement ) &&
			       string.Equals( this.Sku, other.Sku ) &&
			       this.InventoryLevels.Equals( other.InventoryLevels );
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != this.GetType() )
				return false;
			return this.Equals( ( ShopifyProductVariant )obj );
		}
	}

	public enum InventoryManagement
	{
		Blank,
		Shopify
	}
}