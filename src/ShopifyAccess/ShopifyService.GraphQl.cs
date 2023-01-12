using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ShopifyAccess.GraphQl;
using ShopifyAccess.GraphQl.Misc;
using ShopifyAccess.GraphQl.Models.ProductVariantsWithInventoryLevelsReport;
using ShopifyAccess.GraphQl.Services;
using ShopifyAccess.Models;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccess
{
	public sealed partial class ShopifyService
	{
		public async Task< List< ShopifyProductVariant > > GetProductVariantsInventoryReportAsync( CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();
			
			// Log start
			var reportGenerator = new ReportGenerator( this._shopName, this._webRequestServices);
			try
			{
				var data = await reportGenerator.GetReportAsync< ProductVariant >( ReportType.ProductVariantsWithInventoryLevels, this._timeouts[ ShopifyOperationEnum.GetProductsInventory ], token, mark ).ConfigureAwait( false );
				return new List< ShopifyProductVariant >( data.Where(FilterProductVariants).Select( variant =>  variant.ToShopifyProductVariant() ) );
			}
			finally
			{
				 // ToDo: Log end
			}
		}

		private static bool FilterProductVariants( ProductVariant variant )
		{
			return variant.InventoryItem.Tracked && !string.IsNullOrEmpty( variant.Sku );
		}
	}
}