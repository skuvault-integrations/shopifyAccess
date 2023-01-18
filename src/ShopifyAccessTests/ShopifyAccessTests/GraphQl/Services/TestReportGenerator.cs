using System;
using System.Collections.Generic;
using System.IO;
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
			return await this.GenerateReportAsync( type, Mark.Create, CancellationToken.None );
		}

		public async Task< CurrentBulkOperation > GetCurrentBulkOperationAsync()
		{
			return await this.GetCurrentBulkOperationAsync( Mark.Create, CancellationToken.None );
		}

		public async Task< CurrentBulkOperation > GetBulkOperationByIdAsync( string gid )
		{
			return await this.GetBulkOperationByIdAsync( gid, Mark.Create, CancellationToken.None );
		}

		public async Task< IEnumerable< T > > GetReportDocumentAsync< T >( Func< Stream, IEnumerable< T > > parseMethod, string url ) where T : class
		{
			return await this.GetReportDocumentAsync( url, parseMethod, 100000, Mark.Create, CancellationToken.None );
		}
	}
}