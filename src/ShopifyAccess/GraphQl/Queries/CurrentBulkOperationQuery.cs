namespace ShopifyAccess.GraphQl.Queries
{
	internal static class CurrentBulkOperationQuery
	{
		/// <summary>
		/// Get last bulk operation status
		/// </summary>
		internal const string Query =
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
	}
}