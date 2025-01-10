using System;
using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Common
{
	internal class Weight
	{
		[ DataMember( Name = "value" ) ]
		public float? Value{ get; set; }

		[ DataMember( Name = "unit" ) ]
		public string Unit{ get; set; }
		public WeightUnit UnitStandardized =>
			Enum.TryParse( this.Unit, true, out WeightUnit unit ) ? unit : WeightUnit.POUNDS;
	}
}