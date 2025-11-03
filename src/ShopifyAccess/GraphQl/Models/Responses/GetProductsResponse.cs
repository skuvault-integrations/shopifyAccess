using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.Products;
using ShopifyAccess.GraphQl.Models.Products.Extensions;
using ShopifyAccess.Models.Product;
using ProductVariant = ShopifyAccess.GraphQl.Models.ProductVariantsInventory.ProductVariant;

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
		//TODO GUARD-3954 Remove on feature cleanup
		internal static ShopifyProducts ToShopifyProductsLegacy( this List< Products.Product > responseProducts )
		{
			return responseProducts != null
				? new ShopifyProducts
					{ Products = responseProducts.Select( x => x.ToShopifyProductLegacy() ).ToList() }
				: new ShopifyProducts();
		}

		/// <summary>
		/// Convert Products API response payload to SkuVault ShopifyProduct 
		/// </summary>
		/// <param name="responseProducts"></param>
		/// <param name="additionalProductVariants">Dictionary of productId (key), productVariants (value)</param>
		/// <returns></returns>
		//TODO GUARD-3946 Add tests of additionalProductVariants cases
		internal static ShopifyProducts ToShopifyProducts( this List< Product > responseProducts, IDictionary< string, List< ShopifyAccess.GraphQl.Models.Products.ProductVariant > > additionalProductVariants )
		{
			if( responseProducts == null || !responseProducts.Any() )
			{
				return new ShopifyProducts();
			}

			var shopifyProducts = new ShopifyProducts();
			foreach( var product in responseProducts )
			{
				//TODO GUARD-3946 Append or populate additionalProductVariants to each product that has them
				additionalProductVariants.TryGetValue( product.Id, out var productVariants ); 
				shopifyProducts.Products.Add( product.ToShopifyProduct( productVariants ) );
			}
			return shopifyProducts;
		}
	}
}