using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Orders
{
	[ DataContract ]
	public class Money
	{
		[ DataMember( Name = "amount" ) ]
		public decimal Amount{ get; set; }

		[ DataMember( Name = "currencyCode" ) ]
		public string CurrencyCode{ get; set; }
	}
}