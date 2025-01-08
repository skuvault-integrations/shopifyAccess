using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.Products.Extensions;
using ShopifyAccess.Models.Product;

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
		public Nodes< Products.Product > Products{ get; set; }
	}
	
	internal static class GetProductsResponseExtensions
	{
		internal static ShopifyProducts ToShopifyProducts( this GetProductsResponse response )
		{
			var products = response?.Data?.Products?.Items ?? new List< Products.Product >();
			return new ShopifyProducts
			{
				Products = products.Select( x => x.ToShopifyProduct() ).ToList()
			};
		}
	}
}