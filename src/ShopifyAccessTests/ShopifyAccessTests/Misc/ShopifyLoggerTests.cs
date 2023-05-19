using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.Misc;
using ShopifyAccess.Models.Order;
using ShopifyAccess.Models.Product;

namespace ShopifyAccessTests.Misc
{
	public sealed class ShopifyLoggerTests
	{
		private const string UnmaskedJsonString = "{\"id\":\"450789469\",\"order_number\":\"1001\",\"billing_address\":{\"first_name\":\"Bob\"," +
		                                          "\"address1\":\"ChestnutStreet92\",\"phone\":\"+1(502)-459-2181\",\"city\":\"Louisville\",\"zip\":\"40202\"," +
		                                          "\"province\":\"Kentucky\",\"country\":\"UnitedStates\",\"last_name\":\"Norman\",\"address2\":\"\",\"company\":null,}," +
		                                          "\"customer\":{\"email\":\"bob.norman@mail.example.com\"},\"shipping_address\":{\"first_name\":\"Bob\"," +
		                                          "\"address1\":\"ChestnutStreet92\",\"phone\":\"+1(502)-459-2181\",\"city\":\"Louisville\",\"zip\":\"40202\"," +
		                                          "\"province\":\"Kentucky\",\"country\":\"UnitedStates\",\"last_name\":\"Norman\",\"address2\":\"\",\"company\":\"null\"," +
		                                          "\"name\":\"Bob Norman\",\"country_code\":\"US\",\"province_code\":\"KY\",\"latitude\":45.41634,\"longitude\":-75.6868}}";
		private const string MaskedJsonStringByDefaultMask = "{\"id\":\"450789469\",\"order_number\":\"1001\",\"billing_address\":{\"first_name\":\"***\"," +
		                                        "\"address1\":\"***\",\"phone\":\"***\",\"city\":\"***\",\"zip\":\"40202\"," +
		                                        "\"province\":\"***\",\"country\":\"***\",\"last_name\":\"***\",\"address2\":\"***\",\"company\":\"***\"}," +
		                                        "\"customer\":{\"email\":\"***\"},\"shipping_address\":{\"first_name\":\"***\"," +
		                                        "\"address1\":\"***\",\"phone\":\"***\",\"city\":\"***\",\"zip\":\"40202\",\"province\":\"***\",\"country\":\"***\",\"last_name\":\"***\",\"address2\":\"***\",\"company\":\"***\"," +
		                                        "\"name\":\"***\",\"country_code\":\"***\",\"province_code\":\"***\",\"latitude\":\"***\",\"longitude\":\"***\"}}";
		private const string MaskTemplate = "xxx";
		private const string MaskedJsonStringByMaskTemplate = "{\"id\":\"450789469\",\"order_number\":\"1001\",\"billing_address\":{\"first_name\":\"xxx\"," +
		                                               "\"address1\":\"xxx\",\"phone\":\"xxx\",\"city\":\"xxx\",\"zip\":\"40202\"," +
		                                               "\"province\":\"xxx\",\"country\":\"xxx\",\"last_name\":\"xxx\",\"address2\":\"xxx\",\"company\":\"xxx\"}," +
		                                               "\"customer\":{\"email\":\"xxx\"},\"shipping_address\":{\"first_name\":\"xxx\"," +
		                                               "\"address1\":\"xxx\",\"phone\":\"xxx\",\"city\":\"xxx\",\"zip\":\"40202\",\"province\":\"xxx\",\"country\":\"xxx\",\"last_name\":\"xxx\",\"address2\":\"xxx\",\"company\":\"xxx\"," +
		                                               "\"name\":\"xxx\",\"country_code\":\"xxx\",\"province_code\":\"xxx\",\"latitude\":\"xxx\",\"longitude\":\"xxx\"}}";
		
		[ Test ]
		public void MaskPersonalInfoInJson_ReturnsWithMaskedPersonalInfo_WhenJsonStringContainsPersonalInfo()
		{
			// Arrange

			// Act
			var result = ShopifyLogger.MaskPersonalInfoInJson( UnmaskedJsonString );

			// Assert
			result.Should().Be( MaskedJsonStringByDefaultMask );
		}
		
		[ Test ]
		public void MaskPersonalInfoInJson_ReturnsWithMaskedPersonalInfo_WhenJsonStringContainsPersonalInfo_andGivenMaskTemplate()
		{
			// Arrange

			// Act
			var result = ShopifyLogger.MaskPersonalInfoInJson( UnmaskedJsonString, MaskTemplate );

			// Assert
			result.Should().Be( MaskedJsonStringByMaskTemplate );
		}
		
		[ Test ]
		public void MaskPersonalInfoInShopifyOrders_ReturnsWithMaskedPersonalInfo_WhenItCallsForShopifyOrders()
		{
			// Arrange
			
			// Act
			var result = ShopifyLogger.MaskPersonalInfoInShopifyOrders< ShopifyOrders >( UnmaskedJsonString );

			// Assert
			result.Should().Be( MaskedJsonStringByDefaultMask );
		}
		
		[ Test ]
		public void MaskPersonalInfoInShopifyOrders_ReturnsWithOriginalPersonalInfo_WhenItCallsForNonShopifyOrders()
		{
			// Arrange
			
			// Act
			var result = ShopifyLogger.MaskPersonalInfoInShopifyOrders< ShopifyProduct >( UnmaskedJsonString );

			// Assert
			result.Should().Be( UnmaskedJsonString );
		}
	}
}
