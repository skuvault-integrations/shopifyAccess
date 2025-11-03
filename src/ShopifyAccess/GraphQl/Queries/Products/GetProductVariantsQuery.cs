namespace ShopifyAccess.GraphQl.Queries.Products
{
	internal static class GetProductVariantsQuery
	{
		internal const string GetVariantsQueryByProductId =
			@"query getProductVariants($query: String $first: Int, $after: String){
				productVariants(first: $first, after: $after, query: $query) {
					nodes {
						sku
						title
						barcode
						inventoryItem {
							measurement {
								weight {
									value
									unit
								}
							}
						}
						price
						image
						{
							url
						}
						updatedAt
					}
					pageInfo {
						endCursor
						hasNextPage
					}
				}
			}";
	}
}