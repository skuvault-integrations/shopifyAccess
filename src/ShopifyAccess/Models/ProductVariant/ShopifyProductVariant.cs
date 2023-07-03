using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using ShopifyAccess.Models.Product;

namespace ShopifyAccess.Models.ProductVariant
{
	[ DataContract ]
	public class ShopifyProductVariant
	{
		[ DataMember( Name = "id" ) ]
		public long Id{ get; set; }
		
		[ DataMember( Name = "inventory_management" ) ]
		private string RawInventoryManagement{ get; set; }
		
		[ JsonIgnore ]
		public InventoryManagementEnum InventoryManagement
		{
			get
			{
				if ( Enum.TryParse< InventoryManagementEnum >( RawInventoryManagement, out var inventoryManagement ) )
				{
					return inventoryManagement;
				}
				else
				{
					return InventoryManagementEnum.Blank;
				}
			}
			set
			{
				RawInventoryManagement = value.ToString();
			}
		}

		[ DataMember( Name = "sku" ) ]
		public string Sku{ get; set; }

		[ DataMember( Name = "inventory_item_id" ) ]
		public long InventoryItemId{ get; set; }

		[ DataMember( Name = "barcode" ) ]
		public string Barcode{ get; set; }

		[ DataMember( Name = "title" ) ]
		public string Title{ get; set; }

		[ DataMember( Name = "weight" ) ]
		public decimal Weight{ get; set; }

		[ DataMember( Name = "weight_unit" ) ]
		public string WeightUnit{ get; set; }
		public string WeightUnitStandardized
		{
			get { return this.WeightUnit != null ? this.WeightUnit.Replace( "kg", "kgs" ).Replace( "lb", "lbs" ) : "lbs"; }
		}

		[ DataMember( Name = "price" ) ]
		public decimal Price{ get; set; }

		[ DataMember( Name = "updated_at" ) ]
		public DateTime UpdatedAt{ get; set; }

		/// <summary>
		/// The unique numeric identifier for a product's image. The image must be associated to the same product as the variant
		/// </summary>
		[ DataMember( Name = "image_id" ) ]
		public long? ImageId{ get; set; }

		public ShopifyInventoryLevels InventoryLevels{ get; set; }
	}

	public enum InventoryManagementEnum
	{
		Blank,
		Shopify
	}
}