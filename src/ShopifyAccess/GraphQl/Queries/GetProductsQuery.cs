namespace ShopifyAccess.GraphQl.Queries
{
	internal static class GetProductsQuery
	{
		/// <summary>
		/// Query to get products, for the products sync
		/// https://shopify.dev/docs/api/admin-graphql/2024-04/queries/products 
		/// </summary>
		/// <param name="$query">Filter</param>
		/// <param name="$first">Number of products to return</param>
		/// <param name="$after">Cursor for pagination</param>
		//Assumption: No product has more than 250 variants. If not correct, will have to get variants in pages if a product has more than 250
		internal const string Query =
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
								mediaContentType
								preview
								{
									image
									{
										url
									}
								}
							}
						}
						productType
						descriptionHtml
						updatedAt
						createdAt
					}
					pageInfo {
						endCursor
						hasNextPage
					}
				}
			}";
	}
}