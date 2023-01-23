using System;
using ServiceStack;

namespace ShopifyAccess.GraphQl.Queries
{
	internal static class QueryBuilder
	{
		public static string GetCurrentBulkOperationStatusRequest()
		{
			var request = new { query = PrepareRequest( CurrentBulkOperationQuery.Query ) };
			return request.ToJson();
		}

		public static string GetBulkOperationStatusByIdRequest( string gid )
		{
			var query = BulkOperationByIdQuery.Query.Replace( "{gid}", gid );
			var request = new { query = PrepareRequest( query ) };
			return request.ToJson();
		}

		public static string GetProductVariantInventoryBySkuRequest( string sku, string after, int locationsCount )
		{
			var query = PrepareRequest( GetProductVariantInventoryQuery.Query );
			var escapedSku = sku.ToJson();
			var variables = new
			{
				query = $"sku:{escapedSku}",
				locationsCount,
				after
			};

			var request = new { query = PrepareRequest( query ), variables };
			return request.ToJson();
		}
		
		public static string GetReportRequest( ReportType type )
		{
			var query = string.Empty;
			switch( type )
			{
				case ReportType.ProductVariantsInventory:
					query = GetProductVariantsInventoryReportQuery.Query;
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