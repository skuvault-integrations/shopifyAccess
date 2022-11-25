using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace ShopifyAccess.Services
{
	/// <summary>
	/// Verifies Shopify requests
	/// </summary>
	public static class RequestVerificationService
	{
		private const string HMACParamName = "hmac";

		/// <summary>
		/// Verifies the authenticity of Shopify requests on installs SkuVault app
		/// through the Shopify App Store or using an installation link
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

			// compute hmac for query string
			ASCIIEncoding encoding = new ASCIIEncoding();
			var byteQueryString = encoding.GetBytes(queryString);
			var byteClientSecret = encoding.GetBytes(clientSecret);
			var byteQueryStringHash = HashHMAC(byteClientSecret, byteQueryString);
			string queryStringHash = BitConverter.ToString(byteQueryStringHash).Replace("-", "");

			return string.Equals(hmac, queryStringHash, StringComparison.OrdinalIgnoreCase);
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