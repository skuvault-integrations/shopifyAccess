﻿using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.ProductVariantsInventoryReport
{
	[ DataContract ]
	internal class Location
	{
		[ DataMember( Name = "id" ) ]
		public string LocationId{ get; set; }

		[ DataMember( Name = "name" ) ]
		public string Name{ get; set; }
	}
}