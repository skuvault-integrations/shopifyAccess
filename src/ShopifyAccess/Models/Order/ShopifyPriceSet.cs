using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order
{
	[ DataContract ]
	public class ShopifyPriceSet
	{
		[ DataMember( Name = "shopMoney" ) ]
		public ShopifyMoney ShopMoney { get; set; }

		[ DataMember( Name = "presentmentMoney" ) ]
		public ShopifyMoney PresentmentMoney { get; set; }
	}

	[ DataContract ]
	public class ShopifyMoney
	{
		[ DataMember( Name = "amount" ) ]
		public decimal Amount { get; set; }

		[ DataMember( Name = "currencyCode" ) ]
		public string CurrencyCode { get; set; }
	}
}
