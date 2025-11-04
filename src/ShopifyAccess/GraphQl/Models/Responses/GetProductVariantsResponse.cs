using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.Products;

namespace ShopifyAccess.GraphQl.Models.Responses
{
	[ DataContract ]
	internal class GetProductVariantsResponse: GraphQlResponseWithPages< GetProductVariantsData, ProductVariant >
	{
		public override Nodes< ProductVariant > GetItemsAndPagingInfo()
		{
			return this.Data.Product.Variants;
		}
	}

	[ DataContract ]
	internal class GetProductVariantsData
	{
		[ DataMember( Name = "product" ) ]
		public GetProductVariantsInnerData Product{ get; set; }
	}

	[ DataContract ]
	 class GetProductVariantsInnerData
	{
		[ DataMember( Name = "variants" ) ]
		public Nodes< ProductVariant > Variants{ get; set; } = new Nodes< ProductVariant >();
	}
}