using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Location
{
	[ DataContract ]
	public class ShopifyLocations
	{
		[ DataMember( Name = "locations" ) ]
		public List< ShopifyLocation > Locations{ get; private set; }

		public ShopifyLocations()
		{
			this.Locations = new List< ShopifyLocation >();
		}

		public ShopifyLocations( List< ShopifyLocation > locations )
		{
			this.Locations = locations;
		}
	}
}