using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.Products;

namespace ShopifyAccess.GraphQl.Models.Responses
{
	[ DataContract ]
	internal class GetProductVariantsResponse: GraphQlResponseWithPages< GetProductVariantsData, ProductVariantWithProductId >
	{
		public override Nodes< ProductVariantWithProductId > GetItemsAndPagingInfo()
		{
			return this.Data.Variants;
		}
	}

	[ DataContract ]
	internal class GetProductVariantsData
	{
		[ DataMember( Name = "productVariants" ) ]
		public Nodes< ProductVariantWithProductId > Variants{ get; set; } = new Nodes< ProductVariantWithProductId >();
	}
}