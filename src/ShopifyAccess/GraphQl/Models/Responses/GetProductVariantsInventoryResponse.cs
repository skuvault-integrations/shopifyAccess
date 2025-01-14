using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.ProductVariantsInventory;

namespace ShopifyAccess.GraphQl.Models.Responses
{
	[ DataContract ]
	internal class GetProductVariantsInventoryResponse: GraphQlResponseWithPages< GetProductVariantsInventoryData, ProductVariant >
	{
		public override Nodes< ProductVariant > GetItemsAndPagingInfo()
		{
			return this.Data.ProductVariants;
		}
	}

	[ DataContract ]
	internal class GetProductVariantsInventoryData
	{
		[ DataMember( Name = "productVariants" ) ]
		public Nodes< ProductVariant > ProductVariants{ get; set; } = new Nodes< ProductVariant >();
	}
}