using Netco.Logging;
using ShopifyAccess.Models;

namespace ShopifyAccess.Misc
{
	public static class ShopifyLogger
	{
		public static ILogger Log{ get; private set; }

		static ShopifyLogger()
		{
			Log = NetcoLogger.GetLogger( "ShopifyLogger" );
		}

		public static void Trace( Mark mark, string format, params object[] args )
		{
			var markStr = string.Format( "[{0}]\t", mark );
			Log.Trace( markStr + format, args );
		}
	}
}