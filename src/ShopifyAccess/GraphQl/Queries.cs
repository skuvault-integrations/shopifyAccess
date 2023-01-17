using System;
using ServiceStack;

namespace ShopifyAccess.GraphQl
{
	internal static class Queries
	{
		/// <summary>
		/// Get last bulk operation status
		/// </summary>
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

		/// <summary>
		/// Get bulk operation status by gid
		/// </summary>
		private const string BulkOperationById =
			@"query {
				node(id: ""{gid}"") {
					... on BulkOperation {
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
				}
			}";

		/// <summary>
		/// Create Product Variants with inventory levels bulk export operation
		/// </summary>
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

		public static string GetCurrentBulkOperationStatusRequest()
		{
			var request = new { query = PrepareRequest( CurrentBulkOperation ) };
			return request.ToJson();
		}

		public static string GetBulkOperationStatusByIdRequest( string gid )
		{
			var query = BulkOperationById.Replace( "{gid}", gid );
			var request = new { query = PrepareRequest( query ) };
			return request.ToJson();
		}

		public static string GetReportRequest( ReportType type )
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