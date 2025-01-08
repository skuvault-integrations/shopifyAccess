using ShopifyAccess.Models.Product;

namespace ShopifyAccess.GraphQl.Models.Products.Extensions
{
	public static class ProductExtensions
	{
		//TODO GUARD-3717: Add tests
		internal static ShopifyProduct ToShopifyProduct( this Product product )
		{
			return new ShopifyProduct
			{
				Title = product.Title,
				Vendor = product.Vendor,
				//TODO GUARD-3717: Add Images
				//Images = product.Images?.Select( x => x.ToShopifyImage() ).ToList(),
				// 	media(first: 250)
				// 	{
				// 	nodes
				// 	{
				// 	id
				// 	mediaContentType
				// }
				// }
				
				//TODO GUARD-3717: Add Variants
				//Variants = product.Variants?.Select( x => x.ToShopifyProductVariant() ).ToList(),
				// variants(first: 250) {
				// 	nodes {
				// 		id
				// 		sku
				// 		title
				// 		barcode
				// 		inventoryItem {
				// 			measurement {
				// 				weight {
				// 					value
				// 					unit
				// 				}
				// 			}
				// 		}
				// 		price
				// 		image
				// 		{
				// 			id
				// 		}
				// 		updatedAt
				// 	}
				// }
				Type = product.Type,
				BodyHtml = product.DescriptionHtml,
				UpdatedAt = product.UpdatedAt
			};
		}
	}
}