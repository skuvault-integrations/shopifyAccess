using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order
{
	public sealed class ShopifyShippingAddress
	{
		[ DataMember( Name = "zip" ) ]
		public string Zip{ get; set; }
	}
}