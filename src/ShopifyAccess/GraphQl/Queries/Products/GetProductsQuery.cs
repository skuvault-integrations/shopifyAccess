namespace ShopifyAccess.GraphQl.Queries.Products
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
		//Limitation: Only gets 250 variants per product, which can no longer be the case for some tenants (see GUARD-3946)
		//TODO GUARD-3954 Remove on feature cleanup
		internal const string QueryLegacy =
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

		/// <summary>
		/// Query to get products, for the products sync
		/// https://shopify.dev/docs/api/admin-graphql/2025-07/queries/products
		/// </summary>
		/// <param name="$query">Filter</param>
		/// <param name="$first">Number of products to return</param>
		/// <param name="$after">Cursor for pagination</param>
		internal const string Query =
			@"query ($query: String, $first: Int, $after: String) {
				products(query: $query, first: $first, after: $after) {
					nodes {
						title
						id
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