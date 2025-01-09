using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Common
{
	internal class Weight
	{
		[ DataMember( Name = "value" ) ]
		public decimal? Value{ get; set; }

		[ DataMember( Name = "unit" ) ]
		//TODO GUARD-3717: Use enum instead of string
		// KILOGRAMS, GRAMS, POUNDS, OUNCES
		public string Unit{ get; set; }
	}
}