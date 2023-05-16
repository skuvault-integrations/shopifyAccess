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
		[ DataMember( Name = "productVariants" ) ]
		public ProductVariants ProductVariants{ get; set; }
	}
}