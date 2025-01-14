using System;
using System.Threading.Tasks;
using Netco.ThrottlerServices;
using ShopifyAccess.GraphQl.Models;
using ShopifyAccess.Misc;
using ShopifyAccess.Models;

namespace ShopifyAccess.GraphQl
{
	internal class ShopifyGraphQlThrottler
	{
		private const string ThrottledErrorCode = "THROTTLED";

		private readonly string _shopName;
		private readonly int _maxRetryCount;

		public ShopifyGraphQlThrottler( string shopName, int maxRetryCount = 10 )
		{
			this._shopName = shopName;
			this._maxRetryCount = maxRetryCount;
		}

		/// <summary>
		/// Throttle GraphQL request for a single item
		/// </summary>
		/// <param name="funcToThrottle">Function to throttle</param>
		/// <param name="mark">Mark</param>
		/// <typeparam name="TData">GraphQL response "data" element type</typeparam>
		/// <returns>Function response</returns>
		/// <exception cref="T:Netco.ThrottlerServices.ThrottlerException">When throttle max retry count reached</exception>
		public async Task< TData > ExecuteAsync< TData >( Func< Task< BaseGraphQlResponse< TData > > > funcToThrottle, Mark mark ) 
		{
			var retryCount = 1;
			while( true )
			{
				var response = await funcToThrottle().ConfigureAwait( false );
				if( response.Errors == null || response.Errors.Length == 0 )
				{
					await WaitIfNeededAsync( response ).ConfigureAwait( false );
					return response.Data;
				}

				if( response.Errors[ 0 ].Extensions?.Code != ThrottledErrorCode )
				{
					var exception = new SystemException( $"{response.Errors[ 0 ].Message}, Error code: {response.Errors[ 0 ].Extensions?.Code}" );
					ShopifyLogger.LogException( exception, mark, this._shopName );
					throw exception;
				}

				if( retryCount >= this._maxRetryCount )
				{
					var exception = new ThrottlerException( $"Throttle max retry count {this._maxRetryCount} reached" );
					ShopifyLogger.LogException( exception, mark, this._shopName );
					throw exception;
				}

				await WaitIfNeededAsync( response ).ConfigureAwait( false );
				++retryCount;
			}
		}

		/// <summary>
		/// Throttle GraphQL request for paginated response
		/// </summary>
		/// <param name="funcToThrottle">Function to throttle</param>
		/// <param name="mark">Mark</param>
		/// <typeparam name="TData">GraphQL response "data" element type</typeparam>
		/// <typeparam name="TResponseItem"></typeparam>
		/// <returns>Function response</returns>
		/// <exception cref="T:Netco.ThrottlerServices.ThrottlerException">When throttle max retry count reached</exception>
		//TODO GUARD-3717 Add tests
		public async Task< GraphQlResponseWithPages< TData, TResponseItem > > ExecuteWithPaginationAsync< TData, TResponseItem >( Func< Task< GraphQlResponseWithPages< TData, TResponseItem > > > funcToThrottle, Mark mark ) 
		{
			var retryCount = 1;
			while( true )
			{
				var response = await funcToThrottle().ConfigureAwait( false );
				if( response.Errors == null || response.Errors.Length == 0 )
				{
					await WaitIfNeededAsync( response ).ConfigureAwait( false );
					return response;
				}

				if( response.Errors[ 0 ].Extensions?.Code != ThrottledErrorCode )
				{
					var exception = new SystemException( $"{response.Errors[ 0 ].Message}, Error code: {response.Errors[ 0 ].Extensions?.Code}" );
					ShopifyLogger.LogException( exception, mark, this._shopName );
					throw exception;
				}

				if( retryCount >= this._maxRetryCount )
				{
					var exception = new ThrottlerException( $"Throttle max retry count {this._maxRetryCount} reached" );
					ShopifyLogger.LogException( exception, mark, this._shopName );
					throw exception;
				}

				await WaitIfNeededAsync( response ).ConfigureAwait( false );
				++retryCount;
			}
		}

		/// <summary>
		/// Wait for a time needed to restore the quota to execute request
		/// </summary>
		/// <param name="response">GraphQl Response</param>
		/// <returns>Delay in ms</returns>
		public static async Task<int> WaitIfNeededAsync< TData >( BaseGraphQlResponse< TData > response )
		{
			var remainingQuota = response.Extensions.Cost.ThrottleStatus.CurrentlyAvailable;
			var requestedQuota = response.Extensions.Cost.RequestedQueryCost;
			var restoreRete = response.Extensions.Cost.ThrottleStatus.RestoreRate;

			if( remainingQuota > requestedQuota )
			{
				return 0;
			}

			restoreRete = restoreRete > 0 ? restoreRete : 50; // 50 is default restore rate for GraphQl requests
			var delayInSeconds = ( int )Math.Ceiling( 1.0 * ( requestedQuota - remainingQuota ) / restoreRete );
			var delayInMs = delayInSeconds * 1000;
			await Task.Delay( delayInMs );
			return delayInMs;
		}
	}
}