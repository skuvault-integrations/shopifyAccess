namespace ShopifyAccess.GraphQl.Queries
{
	internal static class BulkOperationByIdQuery
	{
		/// <summary>
		/// Get bulk operation status by gid
		/// </summary>
		public const string Query =
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
	}
}