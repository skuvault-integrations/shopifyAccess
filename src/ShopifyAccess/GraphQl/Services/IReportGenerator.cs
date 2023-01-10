using System.Threading;
using System.Threading.Tasks;
using ShopifyAccess.GraphQl.Models.BulkOperation;
using ShopifyAccess.Models;

namespace ShopifyAccess.GraphQl.Services
{
	internal interface IReportGenerator
	{
		/// <summary>
		/// Generates report of a the specified type with specified params.
		/// </summary>
		/// <param name="reportType">Type of report</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <param name="mark">Mark</param>
		/// <returns><see cref="CurrentBulkOperation" /> for the generated report.</returns>
		Task<CurrentBulkOperation> GenerateReportAsync(
			ReportType reportType,
			CancellationToken cancellationToken,
			Mark mark = null);
	}
}