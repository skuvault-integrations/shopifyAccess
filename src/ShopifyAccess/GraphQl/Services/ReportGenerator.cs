using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ShopifyAccess.GraphQl.Misc;
using ShopifyAccess.GraphQl.Models;
using ShopifyAccess.GraphQl.Models.BulkOperation;
using ShopifyAccess.Misc;
using ShopifyAccess.Models;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Services;

namespace ShopifyAccess.GraphQl.Services
{
	internal class ReportGenerator: IReportGenerator
	{
		private const int GraphQlRequestTimeout = 10000;
		private readonly WebRequestServices _webRequestServices;
		private readonly string _shopName;

		private const int MaxGetReportStatusPollingRequests = 30;

		private static int GetReportStatusPollingIntervalMs( int i )
		{
			if( i <= 10 ) return 10000;
			return i <= 20 ? 20000 : 30000;
		}

		public ReportGenerator( string shopName, WebRequestServices webRequestServices )
		{
			this._webRequestServices = webRequestServices;
			this._shopName = shopName;
		}

		public async Task< IEnumerable< T > > GetReportAsync< T >( ReportType reportType, int timeout, CancellationToken cancellationToken, Mark mark = null ) where T : class
		{
			mark = mark.CreateNewIfBlank();

			// Send request to generate report
			var createReportOperation = await this.GenerateReportAsync( reportType, cancellationToken, mark ).ConfigureAwait( false );
			ThrowIfBulkOperationError( createReportOperation, mark );

			// poll for status
			var reportInfo = await this.WaitForReportProcessingAsync( cancellationToken, mark ).ConfigureAwait( false );
			ThrowIfCurrentBulkOperationError( reportInfo, createReportOperation, mark );

			// Download and Parse
			var document = await this.GetReportDocumentAsync< T >( reportType, reportInfo.Url, cancellationToken, mark, timeout ).ConfigureAwait( false );
			return document.As< T >();
		}

		protected async Task< BulkOperation > GenerateReportAsync( ReportType type, CancellationToken cancellationToken, Mark mark )
		{
			var request = Queries.GetReportRequest( type );

			var result = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
				async () => await this._webRequestServices.PostDataAsync< BulkOperationRunQueryResponse >( ShopifyCommand.GraphGl, request, cancellationToken, mark, GraphQlRequestTimeout ).ConfigureAwait( false )
			).ConfigureAwait( false );

			return result.Data?.BulkOperationRunQuery?.BulkOperation;
		}

		protected async Task< CurrentBulkOperation > GetCurrentBulkOperationAsync( CancellationToken cancellationToken, Mark mark )
		{
			var request = Queries.GetCurrentBulkOperationRequest();

			var result = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
				async () => await this._webRequestServices.PostDataAsync< CurrentBulkOperationResponse >( ShopifyCommand.GraphGl, request, cancellationToken, mark, GraphQlRequestTimeout ).ConfigureAwait( false )
			).ConfigureAwait( false );

			return result.Data?.CurrentBulkOperation;
		}

		protected async Task< Report > GetReportDocumentAsync< T >( ReportType reportType, string url, CancellationToken cancellationToken, Mark mark, int timeout ) where T : class
		{
			var parser = GetReportParser( reportType );

			var result = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
				async () => await this._webRequestServices.GetReportDocumentAsync( url, parser, cancellationToken, mark, timeout ).ConfigureAwait( false )
			).ConfigureAwait( false );

			return result;
		}

		private async Task< CurrentBulkOperation > WaitForReportProcessingAsync( CancellationToken cancellationToken, Mark mark )
		{
			var pollingRequestCount = 0;
			CurrentBulkOperation reportInfo;

			// ToDo: Add logs
			try
			{
				do
				{
					cancellationToken.ThrowIfCancellationRequested();

					var delayMs = TimeSpan.FromMilliseconds( GetReportStatusPollingIntervalMs( pollingRequestCount ) );
					await Task.Delay( delayMs, cancellationToken ).ConfigureAwait( false );

					reportInfo = await this.GetCurrentBulkOperationAsync( cancellationToken, mark ).ConfigureAwait( false );
					var isFinalReportStatus = IsFinalReportStatus( reportInfo );
					if( isFinalReportStatus )
					{
						break;
					}
				} while( ++pollingRequestCount < MaxGetReportStatusPollingRequests );
			}
			finally
			{
				// ToDo: Add logs with pollingRequestCount
			}

			return reportInfo;
		}

		private static Func< Stream, Report > GetReportParser( ReportType type )
		{
			switch( type )
			{
				case ReportType.ProductVariantsWithInventoryLevels:
					return ProductVariantsWithInventoryLevelsParser.Parse;
				default:
					throw new ArgumentOutOfRangeException( nameof(type), type, null );
			}
		}

		private static void ThrowIfBulkOperationError( BulkOperation operation, Mark mark )
		{
			if( Enum.TryParse< BulkOperationStatus >( operation?.Status, true, out var status ) )
			{
				if( status == BulkOperationStatus.Created )
				{
					return;
				}
			}

			// ToDo: Add logs;
			// ToDo: Format exception;
			throw new Exception();
		}

		private static void ThrowIfCurrentBulkOperationError( CurrentBulkOperation reportInfo, BulkOperation operation, Mark mark )
		{
			if( Enum.TryParse< BulkOperationStatus >( reportInfo?.Status, true, out var status ) )
			{
				if( status == BulkOperationStatus.Completed && reportInfo.Id.Equals( operation.Id ) )
				{
					return;
				}
			}

			// ToDo: Add logs;
			// ToDo: Format exception;
			throw new Exception();
		}

		private static bool IsFinalReportStatus( CurrentBulkOperation reportInfo )
		{
			if( !Enum.TryParse< BulkOperationStatus >( reportInfo?.Status, true, out var status ) )
				return true;

			switch( status )
			{
				case BulkOperationStatus.Created:
				case BulkOperationStatus.Running:
					return false;
				case BulkOperationStatus.Canceled:
				case BulkOperationStatus.Canceling:
				case BulkOperationStatus.Completed:
				case BulkOperationStatus.Expired:
				case BulkOperationStatus.Failed:
					return true;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}