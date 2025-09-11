using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ShopifyAccess;
using ShopifyAccess.Models.Order;

namespace ShopifyAccessTests.Orders
{
	[ TestFixture ]
	public class OrderTests
	{
		private static readonly Randomizer _randomizer = new Randomizer();
		
		[ Test ]
		public void ProcessRefundOrderLineItems_ReturnLineItemQuantityRefundDeducted_WhenIsRefundQuantityLessThanLineItemQuantity()
		{
			// Arrange
			var quantity = _randomizer.Next( 2, int.MaxValue );
			var refundQuantity = _randomizer.Next( 1, quantity - 1 );
			var shopifyOrder = GenerateShopifyOrderJsonWithRefund( quantity, refundQuantity );
			
			// Act
			ShopifyService.ProcessRefundOrderLineItems( shopifyOrder );	

			// Assert
			Assert.That( shopifyOrder.OrderItems[ 0 ].Quantity, Is.EqualTo( quantity - refundQuantity ) );
		}
		
		[ Test ]
		public void ProcessRefundOrderLineItems_LineItemRemovedFromOrder_WhenIsRefundQuantityEqualsToLineItemQuantity_andRestockTypeIsCancel()
		{
			// Arrange
			var quantity = _randomizer.Next( 2, int.MaxValue );
			var shopifyOrder = GenerateShopifyOrderJsonWithRefund( quantity, quantity, restockType: "cancel" );
			
			// Act
			ShopifyService.ProcessRefundOrderLineItems( shopifyOrder );

			// Assert
			Assert.That( shopifyOrder.OrderItems.Count, Is.EqualTo( 0 ) );
		}

		private static ShopifyOrder GenerateShopifyOrderJsonWithRefund( int orderQuantity, int refundQuantity, string restockType = "" )
		{
			var lineItemId = _randomizer.Next();
			var shopifyOrderWithRefund = new ShopifyOrder()
			{
				OrderItems = new List< ShopifyOrderItem >()
				{
					new ShopifyOrderItem()
					{
						Id = lineItemId.ToString(),
						Quantity = orderQuantity,
					}
				},
				Refunds = new List< ShopifyOrderRefund >()
				{
					new ShopifyOrderRefund()
					{
						RefundLineItems = new List< ShopifyOrderRefundLineItem >()
						{
							new ShopifyOrderRefundLineItem()
							{
								LineItemId = lineItemId,
								Quantity = refundQuantity,
								RestockType = restockType
							}
						}
					}
				}
			};

			return shopifyOrderWithRefund;
		}
	}
}