using System.Runtime.Serialization;
using Netco.Extensions;

namespace ShopifyAccess.GraphQl.Models.Common
{
	internal class Weight
	{
		[ DataMember( Name = "value" ) ]
		public float? Value{ get; set; }

		[ DataMember( Name = "unit" ) ]
		public string Unit{ get; set; }
		public WeightUnit UnitStandardized =>
			this.Unit.ToEnum( WeightUnit.POUNDS );
	}
}