using NUnit.Framework;
using ShopifyAccess.Services.Utils;
using System.Collections.Specialized;

namespace ShopifyAccessTests
{
	[TestFixture]
	public class HMacSignatureUtilsTests
	{
		/// <summary>
		/// Sample of OAuth query string sent by Shopify. 
		/// The request is signed with an HMAC-SHA256 hash function using a secret key as the private key, 
		/// the result of the request is placed in the hmac parameter
		/// see https://shopify.dev/apps/auth/oauth/getting-started#step-2-verify-the-installation-request
		/// </summary>
		private const string ShopifyOAuthQueryStringRequest = "code=0907a61c0c8d55e99db179b68161bc00&hmac=700e2dadb827fcc8609e9d5ce208b2e9cdaab9df07390d2cbca10d7c328fc4bf&shop=some-shop.myshopify.com&state=0.6784241404160823&timestamp=1337178173";
		
		/// <summary>
		/// An example of a secret key used to hash the ShopifyOAuthQueryStringRequest
		/// </summary>
		private const string ShopifyOAuthSecretKey = "hush";

		/// <summary>
		/// Sample of webhook shopify request body sent by Shopify 
		/// Each webhook request includes a base64-encoded X-Shopify-Hmac-SHA256 header (X-Shopify-Hmac-SHA256), 
		/// which is generated using the app's client secret along with the data sent in the request
		/// see https://shopify.dev/apps/webhooks/configuration/https#step-5-verify-the-webhook
		/// </summary>
		private const string ShopifyWebhookBodyRequest = "{\"test\":{\"test2\": \"testing\"}}";

		/// <summary>
		/// Sample of a secret key used to hash the webhook Shopify requests
		/// </summary>
		private const string ShopifyWebhookSecretKey = "sphss_sjdnbsfsdjbgksbgksgb";

		/// <summary>
		/// Sample of a base64-encoded X-Shopify-Hmac-SHA256 webhook shopify request header
		/// </summary>
		private const string ShopifyHeaderHMACParamValue = "RJCzgKmUoGP/Ylod4MhDP+BXTVOaV4NVbdqa3xn9vw4=";

		[Test]
		public void VerifyOAuthRequest_ReturnsTrue_WhenQueryStringIsCorrect()
		{
			// Arrange

			// Act
			var result = HMacSignatureUtils.VerifyOAuthRequest(ShopifyOAuthQueryStringRequest, ShopifyOAuthSecretKey);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void VerifyOAuthRequest_ReturnsFalse_WhenQueryStringIsIncorrect()
		{
			// Arrange

			// Act
			var result = HMacSignatureUtils.VerifyOAuthRequest(ShopifyOAuthQueryStringRequest + "_", ShopifyOAuthSecretKey);

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public void VerifyOAuthRequest_ReturnsFalse_WhenSecretKeyIsIncorrect()
		{
			// Arrange

			// Act
			var result = HMacSignatureUtils.VerifyOAuthRequest(ShopifyOAuthQueryStringRequest, ShopifyOAuthSecretKey + "_");

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public void VerifyWebhookRequest_ReturnsTrue_WhenRequestBody_andHeaderHMACParamName_areCorrect()
		{
			// Arrange
			NameValueCollection headers = new NameValueCollection
			{
				{ 
					HMacSignatureUtils.HeaderHMACParamName, ShopifyHeaderHMACParamValue
				}
			};

			// Act
			var result = HMacSignatureUtils.VerifyWebhookRequest(headers, ShopifyWebhookBodyRequest, ShopifyWebhookSecretKey);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void VerifyWebhookRequest_ReturnsTrue_WhenRequestBodyIsCorrect_andHeaderHMACParamNameIsInLowerCase()
		{
			// Arrange
			NameValueCollection headers = new NameValueCollection
			{
				{
					HMacSignatureUtils.HeaderHMACParamName.ToLower(), ShopifyHeaderHMACParamValue
				}
			};

			// Act
			var result = HMacSignatureUtils.VerifyWebhookRequest(headers, ShopifyWebhookBodyRequest, ShopifyWebhookSecretKey);

			// Assert
			Assert.IsTrue(result);
		}

		[Test]
		public void VerifyWebhookRequest_ReturnsFalse_WhenRequestBodyIsIncorrect()
		{
			// Arrange
			NameValueCollection headers = new NameValueCollection
			{
				{
					HMacSignatureUtils.HeaderHMACParamName, ShopifyHeaderHMACParamValue
				}
			};

			// Act
			var result = HMacSignatureUtils.VerifyWebhookRequest(headers, ShopifyWebhookBodyRequest + "_", ShopifyWebhookSecretKey);

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public void VerifyWebhookRequest_ReturnsFalse_WhenSecretKeyIsIncorrect()
		{
			// Arrange
			NameValueCollection headers = new NameValueCollection
			{
				{
					HMacSignatureUtils.HeaderHMACParamName, ShopifyHeaderHMACParamValue
				}
			};

			// Act
			var result = HMacSignatureUtils.VerifyWebhookRequest(headers, ShopifyWebhookBodyRequest, ShopifyWebhookSecretKey + "_");

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public void VerifyWebhookRequest_ReturnsFalse_WhenShopifyHeaderHMACParamValueIsIncorrect()
		{
			// Arrange
			NameValueCollection headers = new NameValueCollection
			{
				{
					HMacSignatureUtils.HeaderHMACParamName, ShopifyHeaderHMACParamValue + "_"
				}
			};

			// Act
			var result = HMacSignatureUtils.VerifyWebhookRequest(headers, ShopifyWebhookBodyRequest, ShopifyWebhookSecretKey);

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public void VerifyWebhookRequest_ReturnsFalse_WhenShopifyHeaderHMACDoNotExist()
		{
			// Arrange
			NameValueCollection headers = new NameValueCollection();

			// Act
			var result = HMacSignatureUtils.VerifyWebhookRequest(headers, ShopifyWebhookBodyRequest, ShopifyWebhookSecretKey);

			// Assert
			Assert.IsFalse(result);
		}
	}
}