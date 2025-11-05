namespace ShopifyAccess.GraphQl.Queries.Products
{
	internal static class GetProductVariantsQuery
	{
		internal const string Query =
			@"query GetProductVariantsByProductIds($query: String, $first: Int, $after: String) {
				productVariants(first: $first, after: $after, query: $query) {
				 nodes {
						product {
						 id
						}
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
						updatedAt
				 }
				 pageInfo {
					hasNextPage
					endCursor
				 }
				}
			}";
	}
}