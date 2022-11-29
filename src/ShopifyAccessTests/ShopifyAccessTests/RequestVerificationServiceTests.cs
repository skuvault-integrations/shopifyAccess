using NUnit.Framework;
using ShopifyAccess.Services.Utils;

namespace ShopifyAccessTests
{
	[TestFixture]
	public class RequestVerificationServiceTests
	{
		/// <summary>
		/// Sample query string sent by Shopify. 
		/// The request is signed with an HMAC-SHA256 hash function using a secret key as the private key, 
		/// the result of the request is placed in the hmac parameter
		/// see https://shopify.dev/apps/auth/oauth/getting-started#step-2-verify-the-installation-request
		/// </summary>
		private const string ShopifyOAuthQueryStringRequest = "code=0907a61c0c8d55e99db179b68161bc00&hmac=700e2dadb827fcc8609e9d5ce208b2e9cdaab9df07390d2cbca10d7c328fc4bf&shop=some-shop.myshopify.com&state=0.6784241404160823&timestamp=1337178173";
		/// <summary>
		/// The secret key used to hash the ShopifyOAuthQueryStringRequest
		/// </summary>
		private const string SecretKey = "hush";

		[Test]
		public void VerifyOAuthRequest_ReturnsTrue_WhenQueryStringIsCorrect()
		{
			// Arrange

			// Act
			var result = HMacSignatureUtils.VerifyOAuthRequest(ShopifyOAuthQueryStringRequest, SecretKey);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void VerifyOAuthRequest_ReturnsFalse_WhenQueryStringIsIncorrect()
		{
			// Arrange

			// Act
			var result = HMacSignatureUtils.VerifyOAuthRequest(ShopifyOAuthQueryStringRequest + "_", SecretKey);

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public void VerifyOAuthRequest_ReturnsFalse_WhenSecretKeyIsIncorrect()
		{
			// Arrange

			// Act
			var result = HMacSignatureUtils.VerifyOAuthRequest(ShopifyOAuthQueryStringRequest, SecretKey + "_");

			// Assert
			Assert.IsFalse(result);
		}
	}
}