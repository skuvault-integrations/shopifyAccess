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
		public override Nodes< Products.Product > GetItemsAndPagingInfo()
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
		//TODO GUARD-3954 Remove Legacy on feature cleanup
		internal static ShopifyProducts ToShopifyProductsLegacy( this List< Product > responseProducts )
		{
			return responseProducts != null
				? new ShopifyProducts
					{ Products = responseProducts.Select( x => x.ToShopifyProductLegacy() ).ToList() }
				: new ShopifyProducts();
		}

		//TODO GUARD-3946 Add tests
		internal static ShopifyProducts ToShopifyProducts( this List< Products.Product > responseProducts, IDictionary< string, List< ProductVariant > > variants )
		{
			var shopifyProducts = new ShopifyProducts();
			if( responseProducts == null || !responseProducts.Any() )
			{
				return shopifyProducts;
			}

			//No variants
			if( !variants?.Any() ?? true )
			{
				return new ShopifyProducts
					{ Products = responseProducts.Select( x => x.ToShopifyProduct() ).ToList() };
			}

			foreach( var product in responseProducts )
			{
				variants.TryGetValue( product.Id, out var productVariants ); 
				shopifyProducts.Products.Add( product.ToShopifyProduct( productVariants ) );
			}
			return shopifyProducts;
		}
	}
}