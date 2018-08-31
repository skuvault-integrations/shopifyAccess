using System.Collections.Generic;

namespace ShopifyAccess.Models.Product
{
	public class ShopifyInventoryLevelsModel
	{
		public Dictionary< long, List< ShopifyInventoryLevelModel > > InventoryLevels{ get; }

		public ShopifyInventoryLevelsModel()
		{
			this.InventoryLevels = new Dictionary< long, List< ShopifyInventoryLevelModel > >();
		}
	}

	public class ShopifyInventoryLevelModel
	{
		public long LocationId{ get; set; }

		public int Available{ get; set; }

		public string UpdatedAt{ get; set; }

		public ShopifyInventoryLevelModel()
		{
		}

		public ShopifyInventoryLevelModel( ShopifyInventoryLevel data )
		{
			this.LocationId = data.LocationId;
			this.Available = data.Available;
			this.UpdatedAt = data.UpdatedAt;
		}

		public ShopifyInventoryLevel ToShopifyInventoryLevel( long inventoryItemId )
		{
			return new ShopifyInventoryLevel
			{
				InventoryItemId = inventoryItemId,
				LocationId = this.LocationId,
				Available = this.Available,
				UpdatedAt = this.UpdatedAt
			};
		}
	}
}