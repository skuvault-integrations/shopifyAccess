using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Orders
{
	public class Money
	{
		[ DataMember( Name = "amount" ) ]
		public decimal Amount { get; set; }

		[ DataMember( Name = "currencyCode" ) ]
		public string CurrencyCode { get; set; }
	}

	public class MoneyBag
	{
		[ DataMember( Name = "shopMoney" ) ]
		public Money ShopMoney { get; set; }

		[ DataMember( Name = "presentmentMoney" ) ]
		public Money PresentmentMoney { get; set; }
	}
}
