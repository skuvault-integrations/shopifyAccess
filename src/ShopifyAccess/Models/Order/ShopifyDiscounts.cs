using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order.Discounts
{
	[ DataContract ]
	public class ShopifyDiscountCode
	{
		public ShopifyDiscountCode( string typeValue )
		{
			this.TypeValue = typeValue;
		}

		[ DataMember( Name = "code" ) ]
		public string Code{ get; set; }

		[ DataMember( Name = "amount" ) ]
		public decimal Amount{ get; set; }
		
		[ DataMember( Name = "type" ) ]
		private string TypeValue { get; set; }
		public ShopifyDiscountTypeEnum Type
		{ 
			get
			{
				switch( this.TypeValue )
				{
					case "fixed_amount":
						return ShopifyDiscountTypeEnum.FixedAmount;
					case "percentage":
						return ShopifyDiscountTypeEnum.Percentage;
					default:
						return ShopifyDiscountTypeEnum.Undefined;
				}
			} 
		}
	}

	public enum ShopifyDiscountTypeEnum
	{
		Undefined,
		FixedAmount,
		Percentage
	}
}
