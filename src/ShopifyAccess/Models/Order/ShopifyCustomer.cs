// We should not sync PII (personally identifiable information) Customer fields (see GUARD-2660)
//using System.Runtime.Serialization;

//namespace ShopifyAccess.Models.Order
//{
//	public class ShopifyCustomer
//	{
//		[ DataMember( Name = "email" ) ]
//		public string Email { get; set; }
//	}
//}