using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CuttingEdge.Conditions;
using Netco.Extensions;
using ServiceStack;
using ShopifyAccess.Misc;
using ShopifyAccess.Models;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Models.Location;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.Product;
using ShopifyAccess.Models.ProductVariant;
using ShopifyAccess.Models.User;
using ShopifyAccess.Services;
using ShopifyAccess.Services.Utils;

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