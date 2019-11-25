using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order
{
	[ DataContract ]
	public class ShopifyPriceSet
	{
		[ DataMember( Name = "shop_money" ) ]
		public ShopifyMoney ShopMoney { get; set; }

		[ DataMember( Name = "presentment_money" ) ]
		public ShopifyMoney PresentmentMoney { get; set; }
	}

	[ DataContract ]
	public class ShopifyMoney
	{
		[ DataMember( Name = "amount" ) ]
		public decimal Amount { get; set; }

		[ DataMember( Name = "currency_code" ) ]
		public string CurrencyCode { get; set; }
	}
}
