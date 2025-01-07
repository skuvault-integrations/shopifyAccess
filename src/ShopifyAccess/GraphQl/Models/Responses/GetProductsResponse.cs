using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Responses
{
    [ DataContract ]
    internal class GetProductsResponse: BaseGraphQlResponse
    {
        [ DataMember( Name = "data" ) ]
        public GetProductsData Data{ get; set; }
    }

    [ DataContract ]
    internal class GetProductsData
    {
        [ DataMember( Name = "products" ) ]
        public Products.Products Data{ get; set; }
    }
}