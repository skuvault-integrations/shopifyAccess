using System;
using System.Threading.Tasks;
using Netco.ActionPolicyServices;
using Netco.Utils;

namespace ShopifyAccess.Misc
{
	public static class ActionPolicies
	{
#if DEBUG
		private const int RetryCount = 1;
#else
		private const int RetryCount = 50;
#endif

		public static readonly ActionPolicy ShopifyGetPolicy = ActionPolicy.Handle< Exception >().Retry( RetryCount, ( ex, i ) =>
		{
			ShopifyLogger.Log.Trace( ex, "Retrying Shopify API get call for the {0} time", i );
			SystemUtil.Sleep( TimeSpan.FromSeconds( 0.6 ) );
		} );

		public static readonly ActionPolicyAsync ShopifyGetPolicyAsync = ActionPolicyAsync.Handle< Exception >().RetryAsync( RetryCount, async ( ex, i ) =>
		{
			ShopifyLogger.Log.Trace( ex, "Retrying Shopify API get call for the {0} time", i );
			await Task.Delay( TimeSpan.FromSeconds( 0.6 ) );
		} );

		public static readonly ActionPolicy ShopifySubmitPolicy = ActionPolicy.Handle< Exception >().Retry( RetryCount, ( ex, i ) =>
		{
			ShopifyLogger.Log.Trace( ex, "Retrying Shopify API submit call for the {0} time", i );
			SystemUtil.Sleep( TimeSpan.FromSeconds( 0.6 ) );
		} );

		public static readonly ActionPolicyAsync ShopifySubmitPolicyAsync = ActionPolicyAsync.Handle< Exception >().RetryAsync( RetryCount, async ( ex, i ) =>
		{
			ShopifyLogger.Log.Trace( ex, "Retrying Shopify API submit call for the {0} time", i );
			await Task.Delay( TimeSpan.FromSeconds( 0.6 ) );
		} );
	}
}