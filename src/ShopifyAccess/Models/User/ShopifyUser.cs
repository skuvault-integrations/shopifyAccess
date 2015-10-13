using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.Models.User
{
	[ DataContract ]
	public class ShopifyUserWrapper
	{
		[ DataMember( Name = "user" ) ]
		public ShopifyUser User{ get; set; }
	}

	[ DataContract ]
	public class ShopifyUser
	{
		[ DataMember( Name = "id" ) ]
		public long Id{ get; set; }

		[ DataMember( Name = "first_name" ) ]
		public string FirstName{ get; set; }

		[ DataMember( Name = "last_name" ) ]
		public string LastName{ get; set; }

		[ DataMember( Name = "email" ) ]
		public string Email{ get; set; }

		[ DataMember( Name = "url" ) ]
		public string Url{ get; set; }

		[ DataMember( Name = "account_owner" ) ]
		public bool AccountOwner{ get; set; }

		[ DataMember( Name = "permissions" ) ]
		public List< string > Permissions{ get; set; }

		[ DataMember( Name = "user_type" ) ]
		public string UserType{ get; set; }
	}
}