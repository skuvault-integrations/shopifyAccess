using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Core.Configuration
{
	[ DataContract ]
	internal class TokenRequestResult
	{
		[ DataMember( Name = "access_token" ) ]
		public string Token { get; set; }
	}
}