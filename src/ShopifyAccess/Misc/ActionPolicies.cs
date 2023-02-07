using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Netco.ActionPolicyServices;
using Netco.Utils;
using ShopifyAccess.Exceptions;
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

		public static ActionPolicy GetPolicy( Mark mark, string shop, CancellationToken token, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicy.From( ex => ShouldRetry( ex, token ) ).Retry( RetryCount, ( ex, i ) =>
			{
				ShopifyLogger.Trace( ex, mark, "ShopName: {0}\tRetrying Shopify API get call for the {1} time. Caller:{2}.", shop, i, callerName );
				SystemUtil.Sleep( TimeSpan.FromSeconds( 5 + 10 * i ) );
			} );
		}

		public static ActionPolicyAsync GetPolicyAsync( Mark mark, string shop, CancellationToken token, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicyAsync.From( ex => ShouldRetry( ex, token ) ).RetryAsync( RetryCount, async ( ex, i ) =>
			{
				ShopifyLogger.Trace( ex, mark, "ShopName: {0}\tRetrying Shopify API get call for the {1} time. Caller:{2}.", shop, i, callerName );
				await Task.Delay( TimeSpan.FromSeconds( 5 + 10 * i ), token );
			} );
		}

		public static ActionPolicy SubmitPolicy( Mark mark, string shop, CancellationToken token, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicy.From( ex => ShouldRetry( ex, token ) ).Retry( RetryCount, ( ex, i ) =>
			{
				ShopifyLogger.Trace( ex, mark, "ShopName: {0}\tRetrying Shopify API submit call for the {1} time. Caller:{2}.", shop, i, callerName );
				SystemUtil.Sleep( TimeSpan.FromSeconds( 5 + 10 * i ) );
			} );
		}

		public static ActionPolicyAsync SubmitPolicyAsync( Mark mark, string shop, CancellationToken token, [ CallerMemberName ] string callerName = "" )
		{
			return ActionPolicyAsync.From( ex => ShouldRetry( ex, token ) ).RetryAsync( RetryCount, async ( ex, i ) =>
			{
				ShopifyLogger.Trace( ex, mark, "ShopName: {0}\tRetrying Shopify API submit call for the {1} time. Caller:{2}.", shop, i, callerName );
				await Task.Delay( TimeSpan.FromSeconds( 5 + 10 * i ), token );
			} );
		}

		private static bool ShouldRetry( Exception ex, CancellationToken token )
		{
			switch( ex )
			{
				case ShopifyTransientException _:																	// HttpStatusCodes 408, 5xx
				case TaskCanceledException canceledException when canceledException.CancellationToken != token:		// LinkedCancellationToken
					return true;
				default:
					return false;
			}
		}
	}
}