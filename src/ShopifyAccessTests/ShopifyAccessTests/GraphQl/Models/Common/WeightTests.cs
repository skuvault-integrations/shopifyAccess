using NUnit.Framework;
using ShopifyAccess.GraphQl.Models.Common;

namespace ShopifyAccessTests.GraphQl.Models.Common
{
	public class WeightTests
	{
		[ TestCase( "POUNDS", WeightUnit.POUNDS ) ]
		[ TestCase( "GRAMS", WeightUnit.GRAMS ) ]
		[ TestCase( "OUNCES", WeightUnit.OUNCES ) ]
		[ TestCase( "KILOGRAMS", WeightUnit.KILOGRAMS ) ]
		public void UnitStandardized_MapsUnitsCorrectly( string rawUnits, WeightUnit expectedStandardizedUnits )
		{
			var weight = new Weight { Unit = rawUnits };
			
			Assert.That( weight.UnitStandardized, Is.EqualTo( expectedStandardizedUnits ));
		}
		
		[ TestCase( "" ) ]
		[ TestCase( null ) ]
		[ TestCase( "BOB" ) ]
		public void UnitStandardized_WhenUnitsAreUnknown_DefaultsToPounds( string unknownUnits )
		{
			var weight = new Weight { Unit = unknownUnits };
			
			Assert.That( weight.UnitStandardized, Is.EqualTo( WeightUnit.POUNDS ));
		}
	}
}