using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ServiceStack;

namespace ShopifyAccess.GraphQl.Queries
{
	internal static class QueryBuilder
	{
		private const int MaxLocationsCount = 250;
		
		/// <summary>
		/// Build query from embedded resource
		/// </summary>
		/// <param name="queryName"></param>
		/// <returns></returns>
		public static string BuildQuery( string queryName )
		{
			var assembly = Assembly.GetExecutingAssembly();
			var manifestResourceName = assembly.GetManifestResourceNames().FirstOrDefault(rn => rn.EndsWith($"{queryName}.graphql", StringComparison.InvariantCultureIgnoreCase ) );
			return ReadQueryFile( assembly, manifestResourceName ).Result;
		}

		private static Task< string > ReadQueryFile( Assembly assembly, string manifestResourceName )
		{
			Task< string > contents = null;
			using( var stream = assembly.GetManifestResourceStream(manifestResourceName) )
			{
				if( stream != null )
					using( var reader = new StreamReader( stream ) )
					{
						contents = reader.ReadToEndAsync();
					}
			}

			return contents ?? Task.FromResult( string.Empty );
		}

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
			if( locationsCount > MaxLocationsCount )
			{
				throw new ArgumentOutOfRangeException( nameof(locationsCount), locationsCount, "LocationsCount should not be more than " + MaxLocationsCount );
			}

			var query = PrepareRequest( BuildQuery( "getProductVariantBySku") );	//TODO Move this string to a constant
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