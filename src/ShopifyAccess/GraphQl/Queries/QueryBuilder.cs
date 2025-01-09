﻿using System;
using ServiceStack;
using ShopifyAccess.GraphQl.Helpers;

namespace ShopifyAccess.GraphQl.Queries
{
	internal static class QueryBuilder
	{
		/// <summary>
		/// A global Shopify GraphQL limit. If request more than this many items, Shopify will return an error
		/// </summary>
		private const int MaxItemsPerResponse = 250;

		public static string GetCurrentBulkOperationStatusRequest()
		{
			var request = new { query = CleanUpRequest( CurrentBulkOperationQuery.Query ) };
			return request.ToJson();
		}

		public static string GetBulkOperationStatusByIdRequest( string gid )
		{
			var query = BulkOperationByIdQuery.Query.Replace( "{gid}", gid );
			var request = new { query = CleanUpRequest( query ) };
			return request.ToJson();
		}

		public static string GetProductVariantInventoryBySkuRequest( string sku, string after, int locationsCount )
		{
			if( locationsCount > MaxItemsPerResponse )
			{
				throw new ArgumentOutOfRangeException( nameof(locationsCount), locationsCount, "LocationsCount should not be more than " + MaxItemsPerResponse );
			}

			var query = CleanUpRequest( GetProductVariantInventoryQuery.Query );
			var escapedSku = sku.ToJson();
			var variables = new
			{
				query = $"sku:{escapedSku}",
				locationsCount,
				after
			};

			var request = new { query = CleanUpRequest( query ), variables };
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

			var request = new { query = CleanUpRequest( query ) };
			return request.ToJson();
		}
		
		/// <summary>
		/// Create a query to get products created on or after the specified date.
		/// </summary>
		/// <param name="createdAtMinUtc"></param>
		/// <param name="after">Pagination cursor to request the next page</param>
		/// <param name="productsPerPage"></param>
		/// <returns></returns>
		//TODO GUARD-3717: Add tests for this method
		public static string GetProductsCreatedOnOrAfterRequest( DateTime createdAtMinUtc, string after = null, int productsPerPage = MaxItemsPerResponse )
		{
			if( productsPerPage > MaxItemsPerResponse )
			{
				throw new ArgumentOutOfRangeException( nameof(productsPerPage), productsPerPage, $"productsPerPage should not be more than {MaxItemsPerResponse}" );
			}
			
			var variables = new
			{
				query = $"created_at:>='{createdAtMinUtc.ToIso8601()}'",
				after,
				first = productsPerPage
			};
			var request = new { query = CleanUpRequest( GetProductsQuery.CreatedOnOrAfterQuery ), variables };
			return request.ToJson();
		}
		
		/// <summary>
		/// Replace characters that are not allowed in the request
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private static string CleanUpRequest( string request )
		{
			return request.Replace( '\t', ' ' );
		}
	}
}