using System.Collections.Generic;
using System.Linq;
using ShopifyAccess.Models.Product;

namespace ShopifyAccess.GraphQl.Models.Products.Extensions
{
	public static class ProductExtensions
	{
		//TODO GUARD-3717: Add tests
		internal static ShopifyProduct ToShopifyProduct( this Product product )
		{
			var productVariants = product.Variants?.Items ?? new List< ProductVariant >();
			return new ShopifyProduct
			{
				Title = product.Title,
				Vendor = product.Vendor,
				//TODO GUARD-3717: DO LAST Images might not be needed at product level since now each variant has image url
				//	Previously, we just used each SKU's (product variant) image id to then get the image Url from the product.Images list
				//BUT variant SKU "AP-BRAKEPARTS-60-201-013" has "image": null, while the parent product has media image "id": "gid://shopify/MediaImage/40175666921786"
				//   maybe this is an example of a product with just one variant, so the product's image should be used when we sync the variant's SKU
				//	but in order to do this, we'd somehow need to get the image Url by the product's image id. Which seems like another call
				//TODO TRY INITIALLY Test getting images from REST. Different scenarios. See what you see in GraphQL. Maybe ask the dim AI.
				
				//Images = product.Images?.Select( x => x.ToShopifyImage() ).ToList(),
				// 	media(first: 250)
				// 	{
				// 	nodes
				// 	{
				// 	id
				// 	mediaContentType
				// }
				// }
				
				Variants = productVariants.Select( x => x.ToShopifyProductVariant() ).ToList(),
				Type = product.Type,
				BodyHtml = product.DescriptionHtml,
				UpdatedAt = product.UpdatedAt ?? default
			};
		}
	}
}