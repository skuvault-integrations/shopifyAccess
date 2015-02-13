using Netco.Logging;

namespace ShopifyAccess.Misc
{
	public static class ShopifyLogger
	{
		public static ILogger Log{ get; private set; }

		static ShopifyLogger()
		{
			Log = NetcoLogger.GetLogger( "ShopifyLogger" );
		}
	}
}