using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order.Discounts
{
	[ DataContract ]
	public class ShopifyDiscountCode
	{
		[ DataMember( Name = "code" ) ]
		public string Code{ get; set; }

		[ DataMember( Name = "amount" ) ]
		public decimal Amount{ get; set; }
		
		[ DataMember( Name = "type" ) ]
		public ShopifyDiscountTypeEnum Type{ get; set; }
	}

	public enum ShopifyDiscountTypeEnum
	{
		Undefined,
		fixed_amount,
		percentage
	}
}
