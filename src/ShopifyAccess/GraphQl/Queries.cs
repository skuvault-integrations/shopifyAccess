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

		public static string GetCurrentBulkOperationQuery()
		{
			var query = new { query = PrepareRequest( CurrentBulkOperation ) };
			return query.ToJson();
		}
		
		private static string PrepareRequest(string request)
		{
			return request.Replace('\t', ' ');
		}
	}
}