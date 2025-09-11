using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order
{
	[ DataContract ]
	public sealed class ShopifyBillingAddress
	{
		[ DataMember( Name = "zip" ) ]
		public string Zip{ get; set; }
	}
}