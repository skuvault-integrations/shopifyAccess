using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ShopifyAccess.GraphQl.Models.Responses;
using ShopifyAccess.Models;

namespace ShopifyAccess.GraphQl.Services
{
	internal interface IReportGenerator
	{
		/// <summary>
		/// Generates report of a the specified type with specified params.
		/// </summary>
		/// <param name="reportType">Type of report</param>
		/// <param name="parseMethod">Function to parse report</param>
		/// <param name="timeout">Timeout</param>
		/// <param name="mark">Mark</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Collection of report lines</returns>
		Task< IEnumerable< T > > GetReportAsync< T >( ReportType reportType,
			Func< Stream, IEnumerable< T > > parseMethod,
			int timeout,
			Mark mark,
			CancellationToken cancellationToken ) where T : class;
	}
}