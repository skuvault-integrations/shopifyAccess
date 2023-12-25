namespace ShopifyAccess.GraphQl.Queries
{
	internal static class GetProductVariantInventoryQuery
	{
		/// <summary>
		/// Create Get Product Variant with inventory levels request
		/// Params:
		///		$query - query string. Example: "sku:'testSku1'" - to search by SKU
		///		$after - cursor for paginate results. Should be null for the first page
		///		$locationsCount - number of locations. Need to specify this argument to reduce the total query cost.
		///			By this article: https://help.shopify.com/en/manual/locations/setting-up-your-locations the max value is 250 for Shopify Plus subscription plan
		///			It means it's safe to use $locationsCount here as first: param (the first param can't be more than 250)
		/// Result: A page with a single product variant with inventory levels
		/// </summary>
		internal const string Query =
			@"query getProductVariantBySku($query: String, $after: String, $locationsCount: Int){
				productVariants(first: 1, after: $after, query: $query) {
					nodes {
						id
						sku
						inventoryItem {
							id
							tracked
							inventoryLevels(first: $locationsCount) {
								nodes {
									location {
										id
										name
									}
									quantities(names: [""available""]) {
										quantity
									}
								}
							}
						}
					}
					pageInfo {
						endCursor
						hasNextPage
					}
				}
			}";
	}
}