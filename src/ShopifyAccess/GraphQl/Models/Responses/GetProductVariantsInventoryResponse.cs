using System.Collections.Generic;
using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.ProductVariantsInventory;

namespace ShopifyAccess.GraphQl.Models.Responses
{
	[ DataContract ]
	internal class GetProductVariantsInventoryResponse: BaseGraphQlResponseWithItems< GetProductVariantsInventoryData, ProductVariant >
	{
		public override List< ProductVariant > GetItems()
		{
			return this.Data.ProductVariants.Items;
		}
	}

	[ DataContract ]
	internal class GetProductVariantsInventoryData
	{
		[ DataMember( Name = "productVariants" ) ]
		public Nodes< ProductVariant > ProductVariants{ get; set; }
	}
}