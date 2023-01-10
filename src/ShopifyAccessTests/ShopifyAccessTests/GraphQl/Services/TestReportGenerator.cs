using System.Threading;
using System.Threading.Tasks;
using ShopifyAccess.GraphQl;
using ShopifyAccess.GraphQl.Models.BulkOperation;
using ShopifyAccess.GraphQl.Services;
using ShopifyAccess.Models;
using ShopifyAccess.Services;

namespace ShopifyAccessTests.GraphQl.Services
{
	internal class TestReportGenerator: ReportGenerator
	{
		public TestReportGenerator( WebRequestServices webRequestServices ): base( webRequestServices )
		{
		}

		public async Task< BulkOperation > GenerateRequestAsync( ReportType type )
		{
			return await base.GenerateRequestAsync( type, CancellationToken.None, Mark.Create );
		}

		public async Task< CurrentBulkOperation > GetCurrentBulkOperationAsync()
		{
			return await base.GetCurrentBulkOperationAsync( CancellationToken.None, Mark.Create );
		}
	}
}