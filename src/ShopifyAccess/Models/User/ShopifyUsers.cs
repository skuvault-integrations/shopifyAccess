using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.Models.User
{
	[ DataContract ]
	public class ShopifyUsers
	{
		[ DataMember( Name = "users" ) ]
		public List< ShopifyUser > Users{ get; set; }
	}
}