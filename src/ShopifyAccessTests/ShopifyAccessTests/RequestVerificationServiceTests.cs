using NUnit.Framework;
using ShopifyAccess.Services;

namespace ShopifyAccessTests
{
	[TestFixture]
	public class RequestVerificationServiceTests
	{
		private const string SecretKey = "hush";
		private const string ShopifyOAuthQueryStringRequest = "code=0907a61c0c8d55e99db179b68161bc00&hmac=700e2dadb827fcc8609e9d5ce208b2e9cdaab9df07390d2cbca10d7c328fc4bf&shop=some-shop.myshopify.com&state=0.6784241404160823&timestamp=1337178173";

		[Test]
		public void VerifyOAuthRequest_ReturnsTrue_WhenQueryStringIsCorrect()
		{
			// Arrange / Act
			var result = RequestVerificationService.VerifyOAuthRequest(ShopifyOAuthQueryStringRequest, SecretKey);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void VerifyOAuthRequest_ReturnsFalse_WhenQueryStringIsIncorrect()
		{
			// Arrange / Act
			var result = RequestVerificationService.VerifyOAuthRequest(ShopifyOAuthQueryStringRequest + "_", SecretKey);

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public void VerifyOAuthRequest_ReturnsFalse_WhenSecretKeyIsIncorrect()
		{
			// Arrange / Act
			var result = RequestVerificationService.VerifyOAuthRequest(ShopifyOAuthQueryStringRequest, SecretKey + "_");

			// Assert
			Assert.IsFalse(result);
		}
	}
}