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
            Assert.That( result.Id, Is.EqualTo( 123 ) );
            Assert.That( result.OrderNumber, Is.EqualTo( order.Number ) );
            Assert.That( result.Name, Is.EqualTo( order.Name ) );
            Assert.That( result.CreatedAt, Is.EqualTo( order.CreatedAt ) );
            Assert.That( result.Total, Is.EqualTo( 10 ) );
            Assert.That( result.BillingAddress, Is.EqualTo( order.BillingAddress ) );
            Assert.That( result.ShippingAddress, Is.EqualTo( order.ShippingAddress ) );
            Assert.That( result.LocationId, Is.EqualTo( "456" ) );
            Assert.That( result.RawSourceName, Is.EqualTo( order.RawSourceName ) );
            Assert.That( result.OrderItems.Count, Is.EqualTo( order.OrderItems.Items.Count ) );
            Assert.That( result.Fulfillments.Count(), Is.EqualTo( order.Fulfillments.Count() ) );
            Assert.That( result.ShippingLines.Count, Is.EqualTo( order.ShippingLines.Count ) );
            Assert.That( result.DiscountCodes.Count(), Is.EqualTo( order.DiscountCodes.Length ) );
            Assert.That( result.TaxLines.Count(), Is.EqualTo( order.TaxLines.Count() ) );
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
            Assert.That( result.Price, Is.EqualTo( 0 ) );
            Assert.That( result.TotalDiscount, Is.EqualTo( 0 ) );
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
            Assert.That( result.Id, Is.EqualTo( 789 ) );
            Assert.That( result.OrderId, Is.EqualTo( orderId ) );
            Assert.That( result.TrackingCompany, Is.EqualTo( fulfillment.TrackingInfo.FirstOrDefault()?.TrackingCompany ) );
            Assert.That( result.TrackingNumber, Is.EqualTo( fulfillment.TrackingInfo.FirstOrDefault()?.TrackingNumber ) );
            Assert.That( result.TrackingUrl, Is.EqualTo( fulfillment.TrackingInfo.FirstOrDefault()?.TrackingUrl ) );
            Assert.That( result.Items.Count(), Is.EqualTo( fulfillment.FulfillmentLineItems.Items.Count ) );
        }
        
        [Test]
        public void ToShopifyPriceSet_MapsAllProperties()
        {
            // Arrange
            var priceSet = new PriceSet
            {
                ShopMoney = new Money
                {
                    Amount = 25,
                    CurrencyCode = "USD"
                },
                PresentmentMoney = new Money
                {
                    Amount = 30,
                    CurrencyCode = "USD"
                }
            };

            // Act
            var result = priceSet.ToShopifyPriceSet();

            // Assert
            Assert.That(result.ShopMoney.Amount, Is.EqualTo(priceSet.ShopMoney.Amount));
            Assert.That(result.ShopMoney.CurrencyCode, Is.EqualTo(priceSet.ShopMoney.CurrencyCode));
            Assert.That(result.PresentmentMoney.Amount, Is.EqualTo(priceSet.PresentmentMoney.Amount));
            Assert.That(result.PresentmentMoney.CurrencyCode, Is.EqualTo(priceSet.PresentmentMoney.CurrencyCode));
        }
        
        [Test]
        public void ToShopifyMoney_MapsAllProperties()
        {
            // Arrange
            var money = new Money
            {
                Amount = 12,
                CurrencyCode = "USD"
            };

            // Act
            var result = money.ToShopifyMoney();

            // Assert
            Assert.That(result.Amount, Is.EqualTo(money.Amount));
            Assert.That(result.CurrencyCode, Is.EqualTo(money.CurrencyCode));
        }

        private static Order CreateOrder()
        {
            return new Order
            {
                Id = "gid://shopify/Order/123",
                Number = 1001,
                Name = "#1001",
                CreatedAt = DateTime.UtcNow,
                Total = new PriceSet { ShopMoney = new Money { Amount = 10 } },
                OrderItems = new Nodes< OrderItem >
                {
                    Items = new List< OrderItem >
                    {
                        new OrderItem
                        {
                            Id = "item1",
                            Sku = "SKU1",
                            Quantity = 2,
                            Price = new PriceSet { ShopMoney = new Money { Amount = 5 } },
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
                                        OriginalUnitPriceSet = new PriceSet
                                        {
                                            ShopMoney = new Money { Amount = 2 }
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
                        OriginalPriceSet = new PriceSet { ShopMoney = new Money { Amount = 3 } },
                        Code = "STD",
                        Source = "shopify"
                    }
                },
                DiscountCodes = new[] { "discount1" },
                TaxLines = new List< TaxLine >
                {
                    new TaxLine { Title = "taxLine1" }
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
                                OriginalUnitPriceSet = new PriceSet
                                {
                                    ShopMoney = new Money { Amount = 2 }
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
