using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Location
{
	[ DataContract ]
	public class ShopifyLocation
	{
		[ DataMember( Name = "id" ) ]
		public string Id{ get; set; }

		[ DataMember( Name = "name" ) ]
		public string Name{ get; set; }

		[ DataMember( Name = "active" ) ]
		public bool IsActive{ get; set; }
	}
}