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
		/// <summary>
		/// Convert Products API response payload to SkuVault ShopifyProduct 
		/// </summary>
		/// <param name="responseProducts"></param>
		/// <param name="additionalProductVariants">Dictionary of productId (key), productVariants (value)</param>
		/// <returns></returns>
		//TODO GUARD-3946 Add tests of additionalProductVariants cases
		internal static ShopifyProducts ToShopifyProducts( this List< Product > responseProducts, IDictionary< string, List< ProductVariant > > additionalProductVariants )
		{
			//TODO GUARD-3946 Append additionalProductVariants to each product that has them
			return responseProducts != null
				? new ShopifyProducts
					{ Products = responseProducts.Select( x => x.ToShopifyProduct() ).ToList() }
				: new ShopifyProducts();
		}
	}
}