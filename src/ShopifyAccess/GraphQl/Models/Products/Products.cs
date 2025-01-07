using System.Collections.Generic;
using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.ProductVariantsInventory;

namespace ShopifyAccess.GraphQl.Models.Products
{
    [ DataContract ]
    internal class Products
    {
        [ DataMember( Name = "nodes" ) ]
        public List< Product > Nodes{ get; set; }

        [ DataMember( Name = "pageInfo" ) ]
        public PageInfo PageInfo{ get; set; }
    }
}