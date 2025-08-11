using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ShopifyAccess.GraphQl.Models;
using ShopifyAccess.GraphQl.Models.Orders;
using ShopifyAccess.GraphQl.Models.Orders.Extensions;
using ShopifyAccess.Models.Order;

namespace ShopifyAccessTests.GraphQl.Models.Orders.Extensions
{
    [TestFixture]
    public class OrderExtensionsTests
    {

        [ Test ]
        public void ToShopifyOrder_MapsAllPropertiesCorrectly()
        {
            // Arrange
            var order = CreateOrder();

            order.Fulfillments.First().Location = new FulfillmentLocation() { Id = "gid://shopify/Location/456" };

            // Act
            var result = order.ToShopifyOrder();

            // Assert
            Assert.AreEqual( 123, result.Id );
            Assert.AreEqual( order.Number, result.OrderNumber );
            Assert.AreEqual( order.Name, result.Name );
            Assert.AreEqual( order.CreatedAt, result.CreatedAt );
            Assert.AreEqual( 10, result.Total );
            Assert.AreEqual( order.BillingAddress, result.BillingAddress );
            Assert.AreEqual( order.ShippingAddress, result.ShippingAddress );
            Assert.AreEqual( "456", result.LocationId );
            Assert.AreEqual( order.RawSourceName, result.RawSourceName );
            Assert.AreEqual( order.OrderItems.Items.Count, result.OrderItems.Count );
            Assert.AreEqual( order.Fulfillments.Count(), result.Fulfillments.Count() );
            Assert.AreEqual( order.ShippingLines.Count, result.ShippingLines.Count );
            Assert.AreEqual( order.DiscountCodes.Length, result.DiscountCodes.Count() );
            Assert.AreEqual( order.TaxLines.Count(), result.TaxLines.Count() );
            Assert.AreEqual( order.Refunds.Count(), result.Refunds.Count() );
        }

        [ Test ]
        public void ToShopifyOrderItem_HandlesNullValues()
        {
            // Arrange
            var item = new OrderItem
            {
                Id = "123",
                Sku = "sku",
                Quantity = 5,
                Price = null,
                TotalDiscount = null,
                TaxLines = new List< TaxLine >()
            };

            // Act
            var result = item.ToShopifyOrderItem();

            // Assert
            Assert.AreEqual( 0, result.Price );
            Assert.AreEqual( 0, result.TotalDiscount );
        }

        [ Test ]
        public void ToShopifyFulfillment_MapsAllProperties()
        {
            // Arrange
            var orderId = 999;
            var fulfillment = CreateFulfillment();

            // Act
            var result = fulfillment.ToShopifyFulfillment( orderId );

            // Assert
            Assert.AreEqual( 789, result.Id );
            Assert.AreEqual( orderId, result.OrderId );
            Assert.AreEqual( fulfillment.TrackingInfo.FirstOrDefault()?.TrackingCompany, result.TrackingCompany );
            Assert.AreEqual( fulfillment.TrackingInfo.FirstOrDefault()?.TrackingNumber, result.TrackingNumber );
            Assert.AreEqual( fulfillment.TrackingInfo.FirstOrDefault()?.TrackingUrl, result.TrackingUrl );
            Assert.AreEqual( fulfillment.FulfillmentLineItems.Items.Count, result.Items.Count() );
        }

        private static Order CreateOrder()
        {
            return new Order
            {
                Id = "gid://shopify/Order/123",
                Number = 1001,
                Name = "#1001",
                CreatedAt = DateTime.UtcNow,
                Total = new ShopifyPriceSet { ShopMoney = new ShopifyMoney { Amount = 10 } },
                OrderItems = new Nodes< OrderItem >
                {
                    Items = new List< OrderItem >
                    {
                        new OrderItem
                        {
                            Id = "item1",
                            Sku = "SKU1",
                            Quantity = 2,
                            Price = new ShopifyPriceSet { ShopMoney = new ShopifyMoney { Amount = 5 } },
                            Title = "Test Item"
                        }
                    }
                },
                BillingAddress = new ShopifyBillingAddress { Zip = "12345" },
                ShippingAddress = new ShopifyShippingAddress { Zip = "12345" },
                Fulfillments = new List< Fulfillment >
                {
                    new Fulfillment
                    {
                        Id = "gid://shopify/Fulfillment/123",
                        FulfillmentLineItems = new Nodes< FulfillmentLineItem >()
                        {
                            Items = new List< FulfillmentLineItem >
                            {
                                new FulfillmentLineItem
                                {
                                    LineItem = new LineItemDetail
                                    {
                                        Id = "gid://shopify/LineItem/123",
                                        OriginalUnitPriceSet = new ShopifyPriceSet
                                        {
                                            ShopMoney = new ShopifyMoney { Amount = 2 }
                                        },
                                        Quantity = 1,
                                        Sku = "SKU1"
                                    }
                                }
                            }
                        }
                    }
                },
                RawSourceName = "Online Store",
                ShippingLines = new List< ShippingLine >
                {
                    new ShippingLine
                    {
                        Id = "shipline1",
                        Title = "Standard",
                        OriginalPriceSet = new ShopifyPriceSet { ShopMoney = new ShopifyMoney { Amount = 3 } },
                        Code = "STD",
                        Source = "shopify"
                    }
                },
                DiscountCodes = new[] { "discount1" },
                TaxLines = new List< TaxLine >
                {
                    new TaxLine { Title = "taxLine1" }
                },
                Refunds = new List< Refund >
                {
                    new Refund
                    {
                        Id = "gid://shopify/Refund/123",
                        RefundLineItems = new Nodes< RefundLineItem >
                        {
                            Items = new List< RefundLineItem >
                            {
                                new RefundLineItem
                                {
                                    Id = 123,
                                    Quantity = 1,
                                    RestockType = "return",
                                    LineItem = new RefundItem() { Id = 1234 }
                                }
                            }
                        }
                    }
                }
            };
        }

        private static Fulfillment CreateFulfillment()
        {
            return new Fulfillment
            {
                Id = "gid://shopify/Fulfillment/789",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                FulfillmentLineItems = new Nodes< FulfillmentLineItem >
                {
                    Items = new List< FulfillmentLineItem >
                    {
                        new FulfillmentLineItem
                        {
                            LineItem = new LineItemDetail
                            {
                                Id = "gid://shopify/LineItem/123",
                                OriginalUnitPriceSet = new ShopifyPriceSet
                                {
                                    ShopMoney = new ShopifyMoney { Amount = 2 }
                                },
                                Quantity = 2,
                                Sku = "SKU1"
                            }
                        }
                    }
                },
                TrackingInfo = new List< TrackingInfo >()
                {
                    new TrackingInfo()
                    {
                        TrackingCompany = "UPS",
                        TrackingNumber = "123456",
                        TrackingUrl = "http://tracking"
                    }
                }
            };
        }
    }
}
