using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.Products;
using ShopifyAccess.GraphQl.Models.Products.Extensions;
using ShopifyAccess.Models.Product;

namespace ShopifyAccess.GraphQl.Models.Responses
{
	[ DataContract ]
	internal class GetProductsResponse: BaseGraphQlResponseWithItems< GetProductsData, Product >
	{
		//TODO GUARD-3717 Doesn't seem needed
		public override List< Product > GetItems()
		{
			return this.Data.Products.Items;
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