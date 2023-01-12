using System.Threading;
using System.Threading.Tasks;
using ShopifyAccess.GraphQl;
using ShopifyAccess.GraphQl.Models;
using ShopifyAccess.GraphQl.Models.BulkOperation;
using ShopifyAccess.GraphQl.Services;
using ShopifyAccess.Models;
using ShopifyAccess.Services;

namespace ShopifyAccessTests.GraphQl.Services
{
	internal class TestReportGenerator: ReportGenerator
	{
		public TestReportGenerator( string shopName, WebRequestServices webRequestServices ): base( shopName, webRequestServices )
		{
		}

		public async Task< BulkOperation > GenerateRequestAsync( ReportType type )
		{
			return await this.GenerateReportAsync( type, CancellationToken.None, Mark.Create );
		}

		public async Task< CurrentBulkOperation > GetCurrentBulkOperationAsync()
		{
			return await this.GetCurrentBulkOperationAsync( CancellationToken.None, Mark.Create );
		}

		public async Task< Report > GetReportDocumentAsync< T >( ReportType type, string url ) where T : class
		{
			return await this.GetReportDocumentAsync< T >( type, url, CancellationToken.None, Mark.Create, 100000 );
		}
	}
}