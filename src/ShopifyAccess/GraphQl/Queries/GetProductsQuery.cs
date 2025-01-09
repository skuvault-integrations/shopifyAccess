namespace ShopifyAccess.GraphQl.Queries
{
	internal static class GetProductsQuery
	{
		/// <summary>
		/// Query to get products created on or after the specified date
		/// https://shopify.dev/docs/api/admin-graphql/2024-04/queries/products 
		/// </summary>
		/// <param name="$query">Filter</param>
		/// <param name="$first">Number of products to return</param>
		/// <param name="$after">Cursor for pagination</param>
		internal const string CreatedOnOrAfterQuery =
			@"query ($query: String, $first: Int, $after: String) {
				products(query: $query, first: $first, after: $after) {
					nodes {
						title
						variants(first: 250) {
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
									id
									url
								}
								updatedAt
							}
						}
						vendor
						media(first: 250)
						{
							nodes
							{
								id
								mediaContentType
							}
						}
						productType
						descriptionHtml
						updatedAt
					}
					pageInfo {
						endCursor
						hasNextPage
					}
				}
			}";

		//TODO GUARD-3717: Add params to the above to get ?limit=250&created_at_max=2023-06-01T00:00:00.0000000Z&updated_at_min=2023-06-01T00:00:00.0000000Z
		//	TEST to ensure that if I use both created_at:< & updated_at:> then it'll do an AND operation, not an OR. 
	}
}