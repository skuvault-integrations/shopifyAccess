namespace ShopifyAccess.GraphQl.Queries
{
	internal static class GetProductsQuery
	{
		/// <summary>
		/// Query to get products created on or after the specified date 
		/// </summary>
		//TODO GUARD-3717: Replace the hard-coded date with the actual date
		//TODO GUARD-3717: Paginate using the after field (see GetProductVariantsInventoryReportBySkuAsync() for example
		internal const string CreatedOnOrAfterQuery =
			//TODO GUARD-3717: Initial, as suggested by .dev
			//query GetProducts($first: Int!, $createdAtMin: String!) {
			//	products(first: $first, query: "created_at:>=$createdAtMin") {

			//TODO GUARD-3717: Subsequent pages, as suggested by .dev
			// query GetNextProducts($first: Int!, $after: String!) {
			// products(first: $first, after: $after) {
			@"query GetProducts {
				products(first: 250, after: null, query: ""created_at:>='2023-06-01T00:00:00Z'"") {
					nodes {
						title
						variants(first: 250) {
							nodes {
								id
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