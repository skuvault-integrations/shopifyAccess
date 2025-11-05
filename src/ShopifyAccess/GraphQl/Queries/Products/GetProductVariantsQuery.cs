namespace ShopifyAccess.GraphQl.Queries.Products
{
	internal static class GetProductVariantsQuery
	{
		//TODO GUARD-3946 Test
		internal const string Query =
			@"query GetProductVariantsByProductIds($query: String, $first: Int, $after: String) {
				productVariants(first: 250, after: $after, query: $query) {
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