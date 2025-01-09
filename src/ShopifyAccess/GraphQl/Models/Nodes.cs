using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models
{
	[ DataContract ]
	internal class Nodes< T >
	{
		[ DataMember( Name = "nodes" ) ]
		internal List< T > Items{ get; set; }
		
		[ DataMember( Name = "pageInfo" ) ]
		internal PageInfo PageInfo{ get; set; }
	}
}