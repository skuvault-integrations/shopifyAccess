using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order.Discounts
{
	public class ShopifyDiscountCode
	{
		public ShopifyDiscountCode( string typeValue )
		{
			this.TypeValue = typeValue;
		}
		
		public string Code{ get; set; }
		
		public decimal Amount{ get; set; }
		
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
