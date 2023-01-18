using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ShopifyAccess.GraphQl;
using ShopifyAccess.GraphQl.Misc;
using ShopifyAccess.GraphQl.Models.ProductVariantsWithInventoryLevelsReport;
using ShopifyAccess.Misc;
using ShopifyAccess.Models;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccess
{
	public sealed partial class ShopifyService
	{
		public async Task< List< ShopifyProductVariant > > GetProductVariantsInventoryReportAsync( CancellationToken token, Mark mark = null )
		{
			mark = mark.CreateNewIfBlank();

			ShopifyLogger.LogOperationStart( this._shopName, mark );

			try
			{
				var data = await this._reportGenerator.GetReportAsync(
					ReportType.ProductVariantsWithInventoryLevels,
					ProductVariantsWithInventoryLevelsParser.Parse,
					this._timeouts[ ShopifyOperationEnum.GetProductsInventory ],
					token,
					mark ).ConfigureAwait( false );
				return new List< ShopifyProductVariant >( data.Where( FilterProductVariants ).Select( variant => variant.ToShopifyProductVariant() ) );
			}
			finally
			{
				ShopifyLogger.LogOperationEnd( this._shopName, mark );
			}
		}

		private static bool FilterProductVariants( ProductVariant variant )
		{
			return variant.InventoryItem.Tracked && !string.IsNullOrEmpty( variant.Sku );
		}
	}
}