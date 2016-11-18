using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Netco.ActionPolicyServices;
using Netco.ThrottlerServices;
using Netco.Utils;
using ShopifyAccess.Models;

namespace ShopifyAccess.Misc
{
	public static class ActionPolicies
	{
#if DEBUG
		private const int RetryCount = 1;
#else
		private const int RetryCount = 10;
#endif

		public static ActionPolicy GetPolicy( Mark mark, string shop, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicy.From( ex => !( ex is ThrottlerException ) ).Retry( RetryCount, ( ex, i ) =>
			{
				ShopifyLogger.Trace( ex, mark, "ShopName: {0}\tRetrying Shopify API get call for the {1} time. Caller:{2}.", shop, i, callerName );
				SystemUtil.Sleep( TimeSpan.FromSeconds( 5 + 10 * i ) );
			} );
		}

		public static ActionPolicyAsync GetPolicyAsync( Mark mark, string shop, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicyAsync.From( ex => !( ex is ThrottlerException ) ).RetryAsync( RetryCount, async ( ex, i ) =>
			{
				ShopifyLogger.Trace( ex, mark, "ShopName: {0}\tRetrying Shopify API get call for the {1} time. Caller:{2}.", shop, i, callerName );
				await Task.Delay( TimeSpan.FromSeconds( 5 + 10 * i ) );
			} );
		}

		public static ActionPolicy SubmitPolicy( Mark mark, string shop, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicy.From( ex => !( ex is ThrottlerException ) ).Retry( RetryCount, ( ex, i ) =>
			{
				ShopifyLogger.Trace( ex, mark, "ShopName: {0}\tRetrying Shopify API submit call for the {1} time. Caller:{2}.", shop, i, callerName );
				SystemUtil.Sleep( TimeSpan.FromSeconds( 5 + 10 * i ) );
			} );
		}

		public static ActionPolicyAsync SubmitPolicyAsync( Mark mark, string shop, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicyAsync.From( ex => !( ex is ThrottlerException ) ).RetryAsync( RetryCount, async ( ex, i ) =>
			{
				ShopifyLogger.Trace( ex, mark, "ShopName: {0}\tRetrying Shopify API submit call for the {1} time. Caller:{2}.", shop, i, callerName );
				await Task.Delay( TimeSpan.FromSeconds( 5 + 10 * i ) );
			} );
		}
	}
}