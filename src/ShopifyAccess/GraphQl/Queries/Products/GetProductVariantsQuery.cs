namespace ShopifyAccess.GraphQl.Queries.Products
{
	internal static class GetProductVariantsQuery
	{
		/// <summary>
		/// Query to get product variants
		/// https://shopify.dev/docs/api/admin-graphql/2025-07/queries/products
		/// </summary>
		/// <param name="$query">Filter</param>
		/// <param name="$first">Number of variants to return</param>
		/// <param name="$after">Cursor for pagination</param>
		/// <returns>List of variants with parent productId for each</returns>
		internal const string QueryVariantsWithProductId =
			@"query GetProductVariants($query: String, $first: Int, $after: String) {
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