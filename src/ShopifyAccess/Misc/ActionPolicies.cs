using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Netco.ActionPolicyServices;
using Netco.Utils;
using ShopifyAccess.Models;

namespace ShopifyAccess.Misc
{
	public static class ActionPolicies
	{
#if DEBUG
		private const int RetryCount = 1;
#else
		private const int RetryCount = 50;
#endif

		public static ActionPolicy CreateShopifyGetPolicy( Mark mark, string shop, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicy.Handle< Exception >().Retry( RetryCount, ( ex, i ) =>
			{
				ShopifyLogger.Log.Trace( ex, "Retrying Shopify API get call for the {0} time. Mark:{1}, Shop:{2}, Caller:{3}.", i, mark.ToStringSafe(), shop, callerName );
				SystemUtil.Sleep( TimeSpan.FromSeconds( 0.6 ) );
			} );
		}

		public static ActionPolicyAsync CreateShopifyGetPolicyAsync( Mark mark, string shop, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicyAsync.Handle< Exception >().RetryAsync( RetryCount, async ( ex, i ) =>
			{
				ShopifyLogger.Log.Trace( ex, "Retrying Shopify API get call for the {0} time. Mark:{1}, Shop:{2}, Caller:{3}.", i, mark.ToStringSafe(), shop, callerName );
				await Task.Delay( TimeSpan.FromSeconds( 0.6 ) );
			} );
		}

		public static ActionPolicy CreateSubmitPolicy( Mark mark, string shop, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicy.Handle< Exception >().Retry( RetryCount, ( ex, i ) =>
			{
				ShopifyLogger.Log.Trace( ex, "Retrying Shopify API submit call for the {0} time. Mark:{1}, Shop:{2}, Caller:{3}.", i, mark.ToStringSafe(), shop, callerName );
				SystemUtil.Sleep( TimeSpan.FromSeconds( 0.6 ) );
			} );
		}

		public static ActionPolicyAsync CreateShopifySubmitPolicyAsync( Mark mark, string shop, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicyAsync.Handle< Exception >().RetryAsync( RetryCount, async ( ex, i ) =>
			{
				ShopifyLogger.Log.Trace( ex, "Retrying Shopify API submit call for the {0} time. Mark:{1}, Shop:{2}, Caller:{3}.", i, mark.ToStringSafe(), shop, callerName );
				await Task.Delay( TimeSpan.FromSeconds( 0.6 ) );
			} );
		}
	}
}