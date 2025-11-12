using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace ShopifyAccess.Services.Utils
{
	/// <summary>
	/// Verifies Shopify requests
	/// </summary>
	public static class HMacSignatureUtils
	{
		/// <summary>
		/// Shopify OAuth request includes a hmac parameter 
		/// which is generated using the app's client secret along with the data sent in the request
		/// </summary>
		private const string HMACParamName = "hmac";

		/// <summary>
		/// Shopify webhook request includes a base64-encoded X-Shopify-Hmac-SHA256 header, 
		/// which is generated using the app's client secret along with the data sent in the request
		/// see https://shopify.dev/apps/webhooks/configuration/https#step-5-verify-the-webhook
		/// </summary>
		public const string HeaderHMACParamName = "X-Shopify-Hmac-SHA256";

		/// <summary>
		/// Verifies OAuth flow requests authenticity by validating its HMAC
		/// </summary>
		/// <param name="request"></param>
		/// <param name="clientSecret"></param>
		/// <returns></returns>
		public static bool VerifyOAuthRequest(string request, string clientSecret)
		{
			var queryParams = GetParamsFromQueryString(request);

			if (!queryParams.ContainsKey(HMACParamName))
			{
				return false;
			}

			var hmac = queryParams[HMACParamName];

			// remove hmac parameter from query string and sort
			queryParams.Remove(HMACParamName);
			queryParams.OrderBy(f => f.Key);
			var queryString = GetQueryStringFromParams(queryParams);

			// compute a hash from the secretKey and the request body
			byte[] byteHmacHash = GetByteHmacHash(queryString, clientSecret);
			string hmacHash = BitConverter.ToString(byteHmacHash).Replace("-", "");

			// webhook is valid if computed hash matches the hmac parameter
			return string.Equals(hmac, hmacHash, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Verifies that an incoming Shopify webhook request is authentic.
		/// </summary>
		/// <param name="requestHeaders">The request's headers (see Request.Headers in ASP.NET MVC)</param>
		/// <param name="request">The body of the request.</param>
		/// <param name="clientSecret">app's secret key.</param>
		/// <returns>A boolean indicating whether the webhook is authentic or not.</returns>
		public static bool VerifyWebhookRequest(NameValueCollection requestHeaders, string request, string clientSecret)
		{
			// get hmac parameter value from request headers collection
			var requestHeadersKeyValues = requestHeaders.AllKeys.Select(k => new KeyValuePair<string, string>(k, requestHeaders[k]));
			var hmacHeaderValue = requestHeadersKeyValues.FirstOrDefault(kvp => kvp.Key.Equals(HeaderHMACParamName, StringComparison.OrdinalIgnoreCase)).Value;
			if (hmacHeaderValue == null)
			{
				return false;
			}

			// compute a hash from the secretKey and the request body
			byte[] byteHmacHash = GetByteHmacHash(request, clientSecret);
			string hmacHash = Convert.ToBase64String(byteHmacHash);

			// webhook is valid if computed hash matches the header hash
			return string.Equals(hmacHeaderValue, hmacHash, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Compute a bytes hash from the clientSecret and the request body
		/// </summary>
		/// <param name="request"></param>
		/// <param name="clientSecret"></param>
		/// <returns></returns>
		private static byte[] GetByteHmacHash(string request, string clientSecret)
		{
			ASCIIEncoding encoding = new ASCIIEncoding();
			var byteQueryString = encoding.GetBytes(request);
			var byteClientSecret = encoding.GetBytes(clientSecret);
			var byteHmacHash = HashHMAC(byteClientSecret, byteQueryString);
			return byteHmacHash;
		}

		private static byte[] HashHMAC(byte[] key, byte[] message)
		{
			using (var hmac = new HMACSHA256(key))
			{
				var hashValue = hmac.ComputeHash(message);
				return hashValue;
			}
		}

		private static Dictionary<string, string> GetParamsFromQueryString(string urlQuery)
		{
			if (string.IsNullOrWhiteSpace(urlQuery))
			{
				throw new AuthenticationException("Webhook request cannot be null or empty");
			}

			var queryParameters = urlQuery.Split('&')
				.Select(q => q.Split('='))
				.ToDictionary(k => k[0], v => v[1]);

			return queryParameters;
		}

		private static string GetQueryStringFromParams(this IDictionary<string, string> parameters)
		{
			return string.Join("&", parameters.Select(x => $"{x.Key}={x.Value}"));
		}
	}
}