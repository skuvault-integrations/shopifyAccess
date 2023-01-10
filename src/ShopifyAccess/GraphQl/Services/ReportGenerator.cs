using System.Threading;
using System.Threading.Tasks;
using ShopifyAccess.GraphQl.Models.BulkOperation;
using ShopifyAccess.Models;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Services;

namespace ShopifyAccess.GraphQl.Services
{
	internal sealed class ReportGenerator: IReportGenerator
	{
		private const int Timeout = 10000;
		private readonly WebRequestServices _webRequestServices;
		
		public ReportGenerator(WebRequestServices webRequestServices)
		{
			this._webRequestServices = webRequestServices;
		}

		public async Task< CurrentBulkOperation > GenerateReportAsync( ReportType reportType, CancellationToken cancellationToken, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			// Send request to generate report
			
			// poll for status
			// GetCurrentBulkOperationAsync
			
			// Download and Parse
			return null;
		}

		private async Task< CurrentBulkOperation > GetCurrentBulkOperationAsync(CancellationToken cancellationToken, Mark mark)
		{
			var query = Queries.GetCurrentBulkOperationQuery();

			// ToDo: Add policy, throttling etc.
			var result = await this._webRequestServices.PostDataAsync< CurrentBulkOperationResponse >( ShopifyCommand.GraphGl, query, cancellationToken, mark, Timeout );
			return result.Data?.CurrentBulkOperation;
		}
	}
}