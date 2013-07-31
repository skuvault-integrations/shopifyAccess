using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order
{
	public class ShopifyCustomer
	{
		[ DataMember( Name = "email" ) ]
		public string Email { get; set; }
	}
}