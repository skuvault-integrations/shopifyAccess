using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.Products;

namespace ShopifyAccess.GraphQl.Models.Responses
{
	[ DataContract ]
	internal class GetProductVariantsResponse: GraphQlResponseWithPages< GetProductVariantsData, ProductVariant >
	{
		public override Nodes< ProductVariant > GetItemsAndPagingInfo()
		{
			return this.Data.ProductVariants;
		}
	}

	[ DataContract ]
	internal class GetProductVariantsData
	{
		[ DataMember( Name = "productVariants" ) ]
		public Nodes< ProductVariant > ProductVariants{ get; set; } = new Nodes< ProductVariant >();
	}
}