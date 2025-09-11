using System.Linq;
using ServiceStack;
using ShopifyAccess.GraphQl.Helpers;
using ShopifyAccess.GraphQl.Models.Common;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.Order.Discounts;

namespace ShopifyAccess.GraphQl.Models.Orders.Extensions
{
	public static class OrderExtensions
	{
		internal static ShopifyOrder ToShopifyOrder( this Order order )
		{
			var locationId = order.Fulfillments?.FirstOrDefault()?.Location.Id;
			var orderId = GraphQlIdParser.Order.GetId( order.Id );

			return new ShopifyOrder
			{
				Id = orderId,
				OrderNumber = order.Number,
				Name = order.Name,
				CreatedAt = order.CreatedAt,
				Total = order.Total.ShopMoney.Amount,
				OrderItems = order.OrderItems?.Items?.Select( x => x.ToShopifyOrderItem() ).ToList(),
				BillingAddress = order.BillingAddress,
				ShippingAddress = order.ShippingAddress,
				ClosedAt = order.ClosedAt,
				CancelledAt = order.CancelledAt,
				FinancialStatus = order.FinancialStatus,
				FulfillmentStatus = order.FulfillmentStatus,
				LocationId = locationId.IsNullOrEmpty() ? null : GraphQlIdParser.Location.GetId( locationId ).ToString(),
				Fulfillments = order.Fulfillments?.Select( o => o.ToShopifyFulfillment( orderId ) ).ToList(),
				RawSourceName = order.RawSourceName,
				ShippingLines = order.ShippingLines?.Select( s => s.ToShopifyShippingLine() ).ToList(),
				DiscountCodes = order.DiscountCodes?.Select( o => new ShopifyDiscountCode( o ) ),
				TaxLines = order.TaxLines?.Select( t => t.ToShopifyTaxLine() )
			};
		}

		internal static ShopifyOrderItem ToShopifyOrderItem( this OrderItem item )
		{
			return new ShopifyOrderItem
			{
				Id = item.Id,
				Sku = item.Sku,
				Quantity = item.Quantity,
				Price = item.Price?.ShopMoney?.Amount ?? 0,
				Title = item.Title,
				TotalDiscount = item.TotalDiscount?.ShopMoney?.Amount ?? 0,
				TotalDiscountSet = item.TotalDiscount.ToShopifyPriceSet(),
				TaxLines = item.TaxLines?.Select( i => i.ToShopifyTaxLine() )
			};
		}

		internal static ShopifyTaxLine ToShopifyTaxLine( this TaxLine taxLine )
		{
			return new ShopifyTaxLine
			{
				Title = taxLine.Title,
				PriceSet = taxLine.PriceSet.ToShopifyPriceSet(),
			};
		}

		internal static ShopifyFulfillment ToShopifyFulfillment( this Fulfillment fulfillment, long orderId )
		{
			var lineItemDetails = fulfillment.FulfillmentLineItems?.Items?.Select( x => x.LineItem );
			var trackingInfo = fulfillment.TrackingInfo?.LastOrDefault();

			return new ShopifyFulfillment
			{
				Id = GraphQlIdParser.Fulfillment.GetId( fulfillment.Id ),
				CreatedAt = fulfillment.CreatedAt,
				UpdatedAt = fulfillment.UpdatedAt,
				Items = lineItemDetails?.Select( i => i.ToShopifyOrderFulfillmentItem() ),
				OrderId = orderId,
				TrackingCompany = trackingInfo?.TrackingCompany,
				TrackingNumber = trackingInfo?.TrackingNumber,
				TrackingUrl = trackingInfo?.TrackingUrl
			};
		}

		internal static ShopifyOrderFulfillmentItem ToShopifyOrderFulfillmentItem( this LineItemDetail lineItemDetail )
		{
			return new ShopifyOrderFulfillmentItem
			{
				Id = GraphQlIdParser.LineItem.GetId( lineItemDetail.Id ),
				Price = lineItemDetail.OriginalUnitPriceSet?.ShopMoney?.Amount ?? 0,
				Quantity = lineItemDetail.Quantity,
				Sku = lineItemDetail.Sku,
				Weight = new Weight { Value = lineItemDetail.Variant?.InventoryItem?.Measurement?.Weight?.Value }
			};
		}

		internal static ShopifyOrderShippingLine ToShopifyShippingLine( this ShippingLine shippingLine )
		{
			return new ShopifyOrderShippingLine
			{
				Id = shippingLine.Id,
				Title = shippingLine.Title,
				Price = shippingLine.OriginalPriceSet?.ShopMoney?.Amount ?? 0,
				Code = shippingLine.Code,
				Source = shippingLine.Source
			};
		}

		internal static ShopifyPriceSet ToShopifyPriceSet( this PriceSet priceSet )
		{
			if( priceSet == null )
				return null;

			return new ShopifyPriceSet
			{
				ShopMoney = priceSet.ShopMoney?.ToShopifyMoney(),
				PresentmentMoney = priceSet.PresentmentMoney?.ToShopifyMoney()
			};
		}

		internal static ShopifyMoney ToShopifyMoney( this Money money )
		{
			if( money == null )
				return null;

			return new ShopifyMoney
			{
				Amount = money.Amount,
				CurrencyCode = money.CurrencyCode
			};
		}
	}
}