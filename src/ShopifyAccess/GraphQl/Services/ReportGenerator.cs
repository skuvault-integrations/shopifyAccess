using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using ShopifyAccess.GraphQl.Models.Responses;
using ShopifyAccess.GraphQl.Queries;
using ShopifyAccess.Misc;
using ShopifyAccess.Models;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Services;

namespace ShopifyAccess.GraphQl.Services
{
	internal class ReportGenerator: IReportGenerator
	{
		private const int GraphQlRequestTimeoutMs = 30 * 1000;
		private readonly WebRequestServices _webRequestServices;
		private readonly ShopifyGraphQlThrottler _throttler;
		private readonly string _shopName;
		private readonly ShopifyCommandFactory _shopifyCommandFactory;

		private const int MaxGetReportStatusPollingRequests = 30;

		private static int GetReportStatusPollingIntervalMs( int i )
		{
			if( i <= 10 ) return 10000;
			return i <= 20 ? 20000 : 30000;
		}

		public ReportGenerator( string shopName, WebRequestServices webRequestServices, ShopifyCommandFactory shopifyCommandFactory )
		{
			this._shopName = shopName;
			this._webRequestServices = webRequestServices;
			this._shopifyCommandFactory = shopifyCommandFactory;
			this._throttler = new ShopifyGraphQlThrottler( this._shopName );
		}

		public async Task< IEnumerable< T > > GetReportAsync< T >( ReportType reportType, Func< Stream, IEnumerable< T > > parseMethod, int timeout, Mark mark, CancellationToken cancellationToken ) where T : class
		{
			ShopifyLogger.LogOperationStart( this._shopName, mark );

			try
			{
				// It's possible to run only one bulk operation of each type (bulkOperationRunMutation or bulkOperationRunQuery) at a time per shop.
				// Need to check CurrentBulkOperation state to be sure it's not in the Running state before create a new one
				// https://shopify.dev/api/usage/bulk-operations/queries#rate-limits
				// But as we have only one operation for now, we can skip this step 

				// Send request to generate report
				var createReportOperation = await this.GenerateReportAsync( reportType, mark, cancellationToken ).ConfigureAwait( false );
				this.ThrowOnCreateBulkOperationError( createReportOperation, mark );

				// poll for status
				var reportInfo = await this.WaitForReportProcessingAsync( createReportOperation.Id, cancellationToken, mark ).ConfigureAwait( false );
				this.ThrowOnGetBulkOperationResultError( reportInfo, mark );

				// Download and Parse
				var document = await this.GetReportDocumentAsync( reportInfo.Url, parseMethod, timeout, mark, cancellationToken ).ConfigureAwait( false );
				return document;
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		public async Task< BulkOperation > GenerateReportAsync( ReportType type, Mark mark, CancellationToken cancellationToken )
		{
			ShopifyLogger.LogOperationStart( this._shopName, mark );

			try
			{
				var request = QueryBuilder.GetReportRequest( type );

				var result = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
					() => this._throttler.ExecuteAsync(
						() => this._webRequestServices.PostDataAsync< BulkOperationRunQueryResponse >( this._shopifyCommandFactory.CreateGraphQlCommand(), request, cancellationToken, mark, GraphQlRequestTimeoutMs )
						, mark )
				).ConfigureAwait( false );

				return result.Data?.BulkOperationRunQuery?.BulkOperation;
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		public async Task< CurrentBulkOperation > GetCurrentBulkOperationAsync( Mark mark, CancellationToken cancellationToken )
		{
			ShopifyLogger.LogOperationStart( this._shopName, mark );

			try
			{
				var request = QueryBuilder.GetCurrentBulkOperationStatusRequest();

				var result = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
					() => this._throttler.ExecuteAsync(
						() => this._webRequestServices.PostDataAsync< GetCurrentBulkOperationResponse >( this._shopifyCommandFactory.CreateGraphQlCommand(), request, cancellationToken, mark, GraphQlRequestTimeoutMs ),
						mark )
				).ConfigureAwait( false );

				return result.Data?.CurrentBulkOperation;
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		public async Task< CurrentBulkOperation > GetBulkOperationByIdAsync( string gid, Mark mark, CancellationToken cancellationToken )
		{
			Condition.Requires( gid, nameof(gid) ).IsNotNullOrEmpty();

			ShopifyLogger.LogOperationStart( this._shopName, mark );
			try
			{
				var request = QueryBuilder.GetBulkOperationStatusByIdRequest( gid );

				var result = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
					() => this._throttler.ExecuteAsync(
						() => this._webRequestServices.PostDataAsync< GetBulkOperationByIdResponse >( this._shopifyCommandFactory.CreateGraphQlCommand(), request, cancellationToken, mark, GraphQlRequestTimeoutMs )
						, mark )
				).ConfigureAwait( false );

				return result.Data?.BulkOperation;
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		public async Task< IEnumerable< T > > GetReportDocumentAsync< T >( string url, Func< Stream, IEnumerable< T > > parseMethod, int timeout, Mark mark, CancellationToken cancellationToken ) where T : class
		{
			ShopifyLogger.LogOperationStart( this._shopName, mark );

			if( string.IsNullOrWhiteSpace( url ) )
			{
				ShopifyLogger.LogOperation( this._shopName, mark, "URL is empty, an empty array will be returned" );
				return Array.Empty< T >();
			}

			try
			{
				var result = await ActionPolicies.GetPolicyAsync( mark, this._shopName ).Get(
					// Don't need to use throttler here as we just download a .jsonl file by direct URL
					() => this._webRequestServices.GetReportDocumentAsync( url, parseMethod, cancellationToken, mark, timeout )
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
			CurrentBulkOperation reportInfo = null;

			ShopifyLogger.LogOperationStart( this._shopName, mark );

			try
			{
				bool isFinalReportStatus;
				do
				{
					cancellationToken.ThrowIfCancellationRequested();

					var delayMs = TimeSpan.FromMilliseconds( GetReportStatusPollingIntervalMs( pollingRequestCount ) );
					ShopifyLogger.LogOperation( this._shopName, mark, "Waiting for report #" + pollingRequestCount + " / " + ( int )delayMs.TotalSeconds + " seconds" );
					await Task.Delay( delayMs, cancellationToken ).ConfigureAwait( false );

					reportInfo = await this.GetBulkOperationByIdAsync( reportGid, mark, cancellationToken ).ConfigureAwait( false );
					isFinalReportStatus = IsFinalReportStatus( reportInfo );
					if( isFinalReportStatus )
					{
						break;
					}
				} while( ++pollingRequestCount < MaxGetReportStatusPollingRequests );

				if( !isFinalReportStatus )
				{
					var exception = new SystemException( "Reached Max Requests limit but didn't get a final report status. Last status: " + reportInfo?.Status );
					ShopifyLogger.LogException( exception, mark, this._shopName );
					throw exception;
				}
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark, $"Object count: {reportInfo?.ObjectCount}, File size: {reportInfo?.FileSize}, Polling requests count: {pollingRequestCount}" );
			}

			return reportInfo;
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

			var exception = new SystemException( "Unexpected report status: " + operation?.Status );
			ShopifyLogger.LogException( exception, mark, this._shopName );
			throw exception;
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

			var exception = new SystemException( "Unexpected report status: " + reportInfo?.Status + ". Error code: " + reportInfo?.ErrorCode );
			ShopifyLogger.LogException( exception, mark, this._shopName );
			throw exception;
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