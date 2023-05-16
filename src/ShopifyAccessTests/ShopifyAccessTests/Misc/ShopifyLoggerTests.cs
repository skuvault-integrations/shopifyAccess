using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.Misc;

namespace ShopifyAccessTests.Misc
{
	public class ShopifyLoggerTests
	{
		[ Test ]
		public void MaskPersonalInfoInJson_ReturnsWithMaskedPersonalInfo_WhenJsonStringContainsPersonalInfo()
		{
			// Arrange
			var jsonString = "{\"id\":\"450789469\",\"order_number\":\"1001\",\"billing_address\":{\"first_name\":\"Bob\"," +
				"\"address1\":\"ChestnutStreet92\",\"phone\":\"+1(502)-459-2181\",\"city\":\"Louisville\",\"zip\":\"40202\"," +
				"\"province\":\"Kentucky\",\"country\":\"UnitedStates\",\"last_name\":\"Norman\",\"address2\":\"\",\"company\":null,}," +
				"\"customer\":{\"email\":\"bob.norman@mail.example.com\"},\"shipping_address\":{\"first_name\":\"Bob\"," +
				"\"address1\":\"ChestnutStreet92\",\"phone\":\"+1(502)-459-2181\",\"city\":\"Louisville\",\"zip\":\"40202\"," +
				"\"province\":\"Kentucky\",\"country\":\"UnitedStates\",\"last_name\":\"Norman\",\"address2\":\"\",\"company\":\"null\"," +
				"\"name\":\"Bob Norman\",\"country_code\":\"US\",\"province_code\":\"KY\",\"latitude\":45.41634,\"longitude\":-75.6868}}";
			const string mask = "***";

			// Act
			var result = ShopifyLogger.MaskPersonalInfoInJson( jsonString, mask ).Replace( "\r\n", "" ).Replace( " ", "" );

			// Assert
			result.Should().Be( "{\"id\":\"450789469\",\"order_number\":\"1001\",\"billing_address\":{\"first_name\":\"***\"," +
				"\"address1\":\"***\",\"phone\":\"***\",\"city\":\"***\",\"zip\":\"40202\"," +
				"\"province\":\"***\",\"country\":\"***\",\"last_name\":\"***\",\"address2\":\"***\",\"company\":\"***\"}," +
				"\"customer\":{\"email\":\"***\"},\"shipping_address\":{\"first_name\":\"***\"," +
				"\"address1\":\"***\",\"phone\":\"***\",\"city\":\"***\",\"zip\":\"40202\",\"province\":\"***\",\"country\":\"***\",\"last_name\":\"***\",\"address2\":\"***\",\"company\":\"***\"," +
				"\"name\":\"***\",\"country_code\":\"***\",\"province_code\":\"***\",\"latitude\":\"***\",\"longitude\":\"***\"}}" );
		}
	}
}
