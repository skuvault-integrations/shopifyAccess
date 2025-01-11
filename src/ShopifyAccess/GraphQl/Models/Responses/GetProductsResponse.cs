using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.Products;
using ShopifyAccess.GraphQl.Models.Products.Extensions;
using ShopifyAccess.Models.Product;

namespace ShopifyAccess.GraphQl.Models.Responses
{
	[ DataContract ]
	internal class GetProductsResponse: GraphQlResponseWithPages< GetProductsData, Product >
	{
		public override Nodes< Products.Product > GetDataWithPagingInfo()
		{
			return this.Data.Products;
		}
	}

	[ DataContract ]
	internal class GetProductsData
	{
		[ DataMember( Name = "products" ) ]
		public Nodes< Products.Product > Products{ get; set; }
	}
	
	internal static class GetProductsResponseExtensions
	{
		internal static ShopifyProducts ToShopifyProducts( this List< Products.Product > responseProducts )
		{
			return responseProducts != null
				? new ShopifyProducts
					{ Products = responseProducts.Select( x => x.ToShopifyProduct() ).ToList() }
				: new ShopifyProducts();
		}
	}
}