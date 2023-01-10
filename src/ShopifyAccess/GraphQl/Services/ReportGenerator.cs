using System.Threading;
using System.Threading.Tasks;
using ShopifyAccess.GraphQl.Models.BulkOperation;
using ShopifyAccess.Models;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Services;

namespace ShopifyAccess.GraphQl.Services
{
	internal class ReportGenerator: IReportGenerator
	{
		private const int Timeout = 10000;
		private readonly WebRequestServices _webRequestServices;

		public ReportGenerator( WebRequestServices webRequestServices )
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

		protected async Task< BulkOperation > GenerateRequestAsync( ReportType type, CancellationToken cancellationToken, Mark mark )
		{
			var request = Queries.GetReportRequest( type );

			// ToDo: Add policy, throttling etc.
			var result = await this._webRequestServices.PostDataAsync< BulkOperationRunQueryResponse >( ShopifyCommand.GraphGl, request, cancellationToken, mark, Timeout );

			// ToDo: log UserErrors
			return result.Data?.BulkOperationRunQuery?.BulkOperation;
		}

		protected async Task< CurrentBulkOperation > GetCurrentBulkOperationAsync( CancellationToken cancellationToken, Mark mark )
		{
			var request = Queries.GetCurrentBulkOperationRequest();

			// ToDo: Add policy, throttling etc.
			var result = await this._webRequestServices.PostDataAsync< CurrentBulkOperationResponse >( ShopifyCommand.GraphGl, request, cancellationToken, mark, Timeout );
			return result.Data?.CurrentBulkOperation;
		}
	}
}