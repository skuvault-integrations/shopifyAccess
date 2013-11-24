using System;
using System.Threading.Tasks;
using Netco.ActionPolicyServices;
using Netco.Logging;
using Netco.Utils;

namespace ShopifyAccess.Misc
{
	public static class ActionPolicies
	{
		public static ActionPolicy ShopifyGetPolicy
		{
			get { return _shopifyGetPolicy; }
		}

		private static readonly ActionPolicy _shopifyGetPolicy = ActionPolicy.Handle< Exception >().Retry( 50, ( ex, i ) =>
			{
				typeof( ActionPolicies ).Log().Trace( ex, "Retrying Shopify API get call for the {0} time", i );
				SystemUtil.Sleep( TimeSpan.FromSeconds( 0.6 ) );
			} );

		public static ActionPolicy ShopifySubmitPolicy
		{
			get { return _shopifySumbitPolicy; }
		}

		private static readonly ActionPolicy _shopifySumbitPolicy = ActionPolicy.Handle< Exception >().Retry( 50, ( ex, i ) =>
			{
				typeof( ActionPolicies ).Log().Trace( ex, "Retrying Shopify API submit call for the {0} time", i );
				SystemUtil.Sleep( TimeSpan.FromSeconds( 0.6 ) );
			} );

		public static ActionPolicyAsync QueryAsync
		{
			get { return _queryAsync; }
		}

		private static readonly ActionPolicyAsync _queryAsync = ActionPolicyAsync.Handle< Exception >().RetryAsync( 50, async ( ex, i ) =>
			{
				typeof( ActionPolicies ).Log().Trace( ex, "Retrying Shopify API get call for the {0} time", i );
				await Task.Delay( TimeSpan.FromSeconds( 0.6 ) );
			} );
	}
}