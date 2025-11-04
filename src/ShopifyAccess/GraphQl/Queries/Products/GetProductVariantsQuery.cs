namespace ShopifyAccess.GraphQl.Queries.Products
{
	internal static class GetProductVariantsQuery
	{
		internal const string GetVariantsQueryByProductId =
			@"query GetProductVariantsByProductId($productId: ID!, $after: String) {
				product(id: $productId) {
					variants(first: 250, after: $after) {
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
							updatedAt
						}
						pageInfo {
							hasNextPage
							endCursor
						}
					}
				}
			}";
	}
}