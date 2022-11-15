using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Pagination
{
	[ DataContract ]
	public class PageInfo
	{
		[ DataMember( Name = "endCursor" ) ]
		public string EndCursor{ get; set; }

		[ DataMember( Name = "hasNextPage" ) ]
		public bool HasNextPage{ get; set; }
	}
}