using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Orders
{
	[ DataContract ]
	public class PriceSet
	{
		[ DataMember( Name = "shopMoney" ) ]
		public Money ShopMoney{ get; set; }

		[ DataMember( Name = "presentmentMoney" ) ]
		public Money PresentmentMoney{ get; set; }
	}
}