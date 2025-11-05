using Newtonsoft.Json;
using ShopifyAccess.GraphQl.Models.Products;

namespace ShopifyAccess.GraphQl.Models.Responses
{
	internal class GetProductVariantsResponse: GraphQlResponseWithPages< GetProductVariantsData, ProductVariantWithProductId >
	{
		public override Nodes< ProductVariantWithProductId > GetItemsAndPagingInfo()
		{
			return this.Data.Variants;
		}
	}

	internal class GetProductVariantsData
	{
		[ JsonProperty( "productVariants" ) ]
		public Nodes< ProductVariantWithProductId > Variants{ get; set; } = new Nodes< ProductVariantWithProductId >();
	}
}