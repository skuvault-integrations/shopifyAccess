using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.ProductVariantsInventory;

namespace ShopifyAccess.GraphQl.Models.Responses
{
	[ DataContract ]
	internal class GetProductVariantsInventoryResponse: BaseGraphQlResponse
	{
		[ DataMember( Name = "data" ) ]
		public GetProductVariantsInventoryData Data{ get; set; }
	}

	[ DataContract ]
	internal class GetProductVariantsInventoryData
	{
		//TODO GUARD-3717: Use Nodes< ProductVariant > instead of ProductVariants
		//	Then delete class ProductVariants
		[ DataMember( Name = "productVariants" ) ]
		public ProductVariants ProductVariants{ get; set; }
	}
}