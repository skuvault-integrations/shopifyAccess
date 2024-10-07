using NUnit.Framework;
using NUnit.Framework.Internal;
using ServiceStack.Text;
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
			var shopifyOrderJson = GenerateShopifyOrderJsonWithRefund( quantity, refundQuantity );
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( shopifyOrderJson );
			
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
			var shopifyOrderJson = GenerateShopifyOrderJsonWithRefund( quantity, quantity, restockType: "cancel" );
			var shopifyOrder = JsonSerializer.DeserializeFromString< ShopifyOrder >( shopifyOrderJson );
			
			// Act
			ShopifyService.ProcessRefundOrderLineItems( shopifyOrder );

			// Assert
			Assert.That( shopifyOrder.OrderItems.Count, Is.EqualTo( 0 ) );
		}

		private static string GenerateShopifyOrderJsonWithRefund( int orderQuantity, int refundQuantity, string restockType = "" )
		{
			var lineItemId = _randomizer.Next();
			return @"
			{			  
				""line_items"": [
			    {
					""id"": " + lineItemId + @",
					""quantity"": " + orderQuantity + @"
			    }
			  ],
			  ""refunds"": [
			    {
					""refund_line_items"": [
			        {
						""line_item_id"": " + lineItemId + @",
						""quantity"": " + refundQuantity + @",
						""restock_type"": " + restockType + @"
			        }
			      ]
			    }
			  ]
			}";
		}
	}
}