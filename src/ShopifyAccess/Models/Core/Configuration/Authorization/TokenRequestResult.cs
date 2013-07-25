using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Core.Configuration.Authorization
{
	[ DataContract ]
	internal class TokenRequestResult
	{
		[ DataMember( Name = "access_token" ) ]
		public string Token { get; set; }
	}
}