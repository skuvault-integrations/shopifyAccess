using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.Misc;

namespace ShopifyAccessTests
{
	public class ExtensionsTests
	{
		[ Test ]
		public void GivenNullUrl_WhenGetUrlWithoutQueryPartIsCalled_ThenNullIsExpected()
		{
			string url = null;
			var urlWithoutQueryPart = url.GetUrlWithoutQueryPart();
			urlWithoutQueryPart.Should().BeNull();
		}

		[ Test ]
		public void GivenWhitespaceUrl_WhenGetUrlWithoutQueryPartIsCalled_ThenNullIsExpected()
		{
			string url = "";
			var urlWithoutQueryPart = url.GetUrlWithoutQueryPart();
			urlWithoutQueryPart.Should().BeEmpty();
		}

		[ Test ]
		public void GivenNotValidUrl_WhenGetUrlWithoutQueryPartIsCalled_ThenSameUrlIsExpected()
		{
			var notValidUrl = "https://skuvault$$.com::";
			var urlWithoutQueryPart = notValidUrl.GetUrlWithoutQueryPart();
			urlWithoutQueryPart.Should().Be( notValidUrl );
		}

		[ Test ]
		public void GivenUrlWithQueryPart_WhenGetUrlWithoutQueryPartIsCalled_ThenUrlWithoutQueryPartIsExpected()
		{
			var urlWithQueryPart = "https://cdn.shopify.com/s/files/5/1077/9210/02010/products/1.jpg?v=1604448440";
			var urlWithoutQueryPart = urlWithQueryPart.GetUrlWithoutQueryPart();
			urlWithoutQueryPart.Should().Be( "https://cdn.shopify.com/s/files/5/1077/9210/02010/products/1.jpg" );
		}

		[ Test ]
		public void GivenUrlWithLongQueryPart_WhenGetUrlWithoutQueryPartIsCalled_ThenUrlWithoutQueryPartIsExpected()
		{
			var urlWithQueryPart = "https://cdn.shopify.com/s/files/5/1077/9210/02010/products/1.jpg?v=1604448440&nonce=292929";
			var urlWithoutQueryPart = urlWithQueryPart.GetUrlWithoutQueryPart();
			urlWithoutQueryPart.Should().Be( "https://cdn.shopify.com/s/files/5/1077/9210/02010/products/1.jpg" );
		}

		[ Test ]
		public void GivenValidUrlWithoutQueryPart_WhenGetUrlWithoutQueryPartIsCalled_ThenUrlWithoutQueryPartIsExpected()
		{
			var imageUrl = "https://cdn.shopify.com/s/files/5/1077/9210/02010/products/1.jpg";
			var urlWithoutQueryPart = imageUrl.GetUrlWithoutQueryPart();
			urlWithoutQueryPart.Should().Be( imageUrl );
		}
	}
}