using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
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

			ShopifyLogger.LogOperationStart( this._shopName, mark );

			try
			{
				// Send request to generate report
				var createReportOperation = await this.GenerateReportAsync( reportType, cancellationToken, mark ).ConfigureAwait( false );
				this.ThrowOnCreateBulkOperationError( createReportOperation, mark );

				// poll for status
				var reportInfo = await this.WaitForReportProcessingAsync( createReportOperation.Id, cancellationToken, mark ).ConfigureAwait( false );
				this.ThrowOnGetBulkOperationResultError( reportInfo, mark );

				// Download and Parse
				var document = await this.GetReportDocumentAsync( reportType, reportInfo.Url, cancellationToken, mark, timeout ).ConfigureAwait( false );
				return document.As< T >();
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		protected async Task< BulkOperation > GenerateReportAsync( ReportType type, CancellationToken cancellationToken, Mark mark )
		{
			ShopifyLogger.LogOperationStart( this._shopName, mark );

			try
			{
				var request = Queries.GetReportRequest( type );

				var result = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
					async () => await this._webRequestServices.PostDataAsync< BulkOperationRunQueryResponse >( ShopifyCommand.GraphGl, request, cancellationToken, mark, GraphQlRequestTimeout ).ConfigureAwait( false )
				).ConfigureAwait( false );

				return result.Data?.BulkOperationRunQuery?.BulkOperation;
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		protected async Task< CurrentBulkOperation > GetCurrentBulkOperationAsync( CancellationToken cancellationToken, Mark mark )
		{
			ShopifyLogger.LogOperationStart( this._shopName, mark );

			try
			{
				var request = Queries.GetCurrentBulkOperationStatusRequest();

				var result = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
					async () => await this._webRequestServices.PostDataAsync< GetCurrentBulkOperationResponse >( ShopifyCommand.GraphGl, request, cancellationToken, mark, GraphQlRequestTimeout ).ConfigureAwait( false )
				).ConfigureAwait( false );

				return result.Data?.CurrentBulkOperation;
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		protected async Task< CurrentBulkOperation > GetBulkOperationByIdAsync( string gid, CancellationToken cancellationToken, Mark mark )
		{
			Condition.Requires( gid, nameof(gid) ).IsNotNullOrEmpty();

			ShopifyLogger.LogOperationStart( this._shopName, mark );
			try
			{
				var request = Queries.GetBulkOperationStatusByIdRequest( gid );

				var result = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
					async () => await this._webRequestServices.PostDataAsync< GetBulkOperationByIdResponse >( ShopifyCommand.GraphGl, request, cancellationToken, mark, GraphQlRequestTimeout ).ConfigureAwait( false )
				).ConfigureAwait( false );

				return result.Data?.BulkOperation;
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		protected async Task< Report > GetReportDocumentAsync( ReportType reportType, string url, CancellationToken cancellationToken, Mark mark, int timeout )
		{
			Condition.Requires( url, nameof(url) ).IsNotNullOrEmpty();

			ShopifyLogger.LogOperationStart( this._shopName, mark );

			try
			{
				var parser = GetReportParser( reportType );

				var result = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
					async () => await this._webRequestServices.GetReportDocumentAsync( url, parser, cancellationToken, mark, timeout ).ConfigureAwait( false )
				).ConfigureAwait( false );

				return result;
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		private async Task< CurrentBulkOperation > WaitForReportProcessingAsync( string reportGid, CancellationToken cancellationToken, Mark mark )
		{
			var pollingRequestCount = 0;
			CurrentBulkOperation reportInfo;

			ShopifyLogger.LogOperationStart( this._shopName, mark );

			try
			{
				do
				{
					cancellationToken.ThrowIfCancellationRequested();

					var delayMs = TimeSpan.FromMilliseconds( GetReportStatusPollingIntervalMs( pollingRequestCount ) );
					ShopifyLogger.LogOperation( this._shopName, mark, "Waiting for report #" + pollingRequestCount + " / " + ( int )delayMs.TotalSeconds + " seconds" );
					await Task.Delay( delayMs, cancellationToken ).ConfigureAwait( false );

					reportInfo = await this.GetBulkOperationByIdAsync( reportGid, cancellationToken, mark ).ConfigureAwait( false );
					var isFinalReportStatus = IsFinalReportStatus( reportInfo );
					if( isFinalReportStatus )
					{
						break;
					}
				} while( ++pollingRequestCount < MaxGetReportStatusPollingRequests );
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
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

		private void ThrowOnCreateBulkOperationError( BulkOperation operation, Mark mark )
		{
			if( Enum.TryParse< BulkOperationStatus >( operation?.Status, true, out var status ) )
			{
				if( status == BulkOperationStatus.Created )
				{
					return;
				}
			}

			ShopifyLogger.LogOperation( this._shopName, mark, "Unexpected report status: " + operation?.Status );
			throw new SystemException( "Unexpected report status: " + operation?.Status );
		}

		private void ThrowOnGetBulkOperationResultError( CurrentBulkOperation reportInfo, Mark mark )
		{
			if( Enum.TryParse< BulkOperationStatus >( reportInfo?.Status, true, out var status ) )
			{
				if( status == BulkOperationStatus.Completed )
				{
					return;
				}
			}

			ShopifyLogger.LogOperation( this._shopName, mark, "Unexpected report status: " + reportInfo?.Status );
			throw new SystemException( "Unexpected report status: " + reportInfo?.Status );
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