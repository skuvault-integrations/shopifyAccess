using System.Threading;
using System.Threading.Tasks;
using ShopifyAccess.Models;
using ShopifyAccess.Models.Product;

namespace ShopifyAccess
{
	public sealed partial class ShopifyService
	{
		public Task< ShopifyProducts > GetProductVariantsInventoryReportAsync( CancellationToken token, Mark mark = null )
		{
			// Log start

			try
			{
				
				// Generate report
				
				//Download report

			}
			finally
			{
				 // ToDo: Log end
			}
			return null;
		}
	}
}