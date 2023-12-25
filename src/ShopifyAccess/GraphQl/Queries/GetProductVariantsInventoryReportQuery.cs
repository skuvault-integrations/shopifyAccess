namespace ShopifyAccess.GraphQl.Queries
{
	internal static class GetProductVariantsInventoryReportQuery
	{
		/// <summary>
		/// Create Product Variants with inventory levels bulk export operation
		/// Here is an example how to write bulk operations
		/// https://shopify.dev/api/usage/bulk-operations/queries#write-a-bulk-operation
		/// </summary>
		internal const string Query =
			@"mutation {
			  bulkOperationRunQuery(
			    query: """"""
					{
						productVariants{
							edges{
								node {
									id
									sku
									inventoryItem {
										id
										tracked
										inventoryLevels{
											edges{
												node {
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
								}
							}
						}
					}
				""""""
			  ) {
			    bulkOperation {
			      id
			      status
			    }
			    userErrors {
			      field
			      message
			    }
			  }
			}";
	}
}