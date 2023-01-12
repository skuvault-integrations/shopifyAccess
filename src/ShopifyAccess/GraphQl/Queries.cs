using System;
using ServiceStack;

namespace ShopifyAccess.GraphQl
{
	internal class Queries
	{
		private const string CurrentBulkOperation =
			@"query {
			  currentBulkOperation {
			    id
			    status
			    errorCode
			    createdAt
			    completedAt
			    objectCount
			    fileSize
			    url
			    partialDataUrl
			  }
			}";

		private const string GetProductVariantsWithInventoryLevelsReport =
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
													available
													location {
														id
														name
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

		public static string GetCurrentBulkOperationRequest()
		{
			var request = new { query = PrepareRequest( CurrentBulkOperation ) };
			return request.ToJson();
		}
		
		public static string GetReportRequest(ReportType type)
		{
			var query = string.Empty;
			switch( type )
			{
				case ReportType.ProductVariantsWithInventoryLevels:
					query = GetProductVariantsWithInventoryLevelsReport;
					break;
				default:
					throw new ArgumentOutOfRangeException( nameof(type), type, null );
			}
			var request = new { query = PrepareRequest( query ) };
			return request.ToJson();
		}

		private static string PrepareRequest( string request )
		{
			return request.Replace( '\t', ' ' );
		}
	}
}