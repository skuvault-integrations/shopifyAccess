using Netco.ThrottlerServices;

namespace ShopifyAccess.Misc
{
	public class ShopifyThrottler: Throttler
	{
		/// <summary>
		/// Throttler constructor. See code section for details
		/// </summary>
		/// <code>
		/// // Maximum request quota: 20 requests. Restore rate: Five items every second
		/// var throttler = new Throttler( 20, 1, 5 )
		/// 
		/// // Maximum request quota of six and a restore rate of one request every minute
		/// var throttler = new Throttler( 6, 60, 1 )
		/// </code>
		/// <param name="maxQuota">Max quota</param>
		/// <param name="delayInSecondsBeforeRelease">Delay in seconds before release</param>
		/// <param name="itemsCountForRelease">Items count for release. Default is 1</param>
		/// <param name="maxRetryCount">Max Retry Count</param>
		public ShopifyThrottler( int maxQuota = 40, int delayInSecondsBeforeRelease = 1, int itemsCountForRelease = 2, int maxRetryCount = 30 ):
			base( maxQuota, delayInSecondsBeforeRelease, itemsCountForRelease, maxRetryCount, "Too Many Requests" )
		{
		}
	}

	public class ShopifyThrottlerAsync: ThrottlerAsync
	{
		/// <summary>
		/// Throttler constructor. See code section for details
		/// </summary>
		/// <code>
		/// // Maximum request quota: 20 requests. Restore rate: Five items every second
		/// var throttler = new Throttler( 20, 1, 5 )
		/// 
		/// // Maximum request quota of six and a restore rate of one request every minute
		/// var throttler = new Throttler( 6, 60, 1 )
		/// </code>
		/// <param name="maxQuota">Max quota</param>
		/// <param name="delayInSecondsBeforeRelease">Delay in seconds before release</param>
		/// <param name="itemsCountForRelease">Items count for release. Default is 1</param>
		/// <param name="maxRetryCount">Max Retry Count</param>
		public ShopifyThrottlerAsync( int maxQuota = 40, int delayInSecondsBeforeRelease = 1, int itemsCountForRelease = 2, int maxRetryCount = 30 ):
			base( maxQuota, delayInSecondsBeforeRelease, itemsCountForRelease, maxRetryCount, "Too Many Requests" )
		{
		}
	}
}