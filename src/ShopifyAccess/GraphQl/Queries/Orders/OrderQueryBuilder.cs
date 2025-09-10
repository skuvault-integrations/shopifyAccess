using System;
using ServiceStack;
using ShopifyAccess.GraphQl.Helpers;

namespace ShopifyAccess.GraphQl.Queries.Orders
{
	public static class OrderQueryBuilder
	{
		public static string GetOrdersRequest(
			DateTime dateFromUtc,
			DateTime dateToUtc,
			string status = null,
			string after = null,
			int ordersPerPage = QueryBuilder.MaxItemsPerResponse )
		{
			if( ordersPerPage > QueryBuilder.MaxItemsPerResponse )
			{
				throw new ArgumentOutOfRangeException( nameof(ordersPerPage), ordersPerPage, $"ordersPerPage should not be greater than {QueryBuilder.MaxItemsPerResponse}" );
			}

			var variables = new
			{
				query = $"created_at:>='{dateFromUtc.ToIso8601()}' created_at:<='{dateToUtc.ToIso8601()}' status:'{status}'",
				after,
				first = ordersPerPage
			};

			var request = new
			{
				query = QueryBuilder.CleanUpRequest( GetOrdersQuery.Query ),
				variables
			};

			return request.ToJson();
		}
	}
}
