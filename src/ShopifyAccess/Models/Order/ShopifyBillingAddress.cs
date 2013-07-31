using System.Runtime.Serialization;

namespace ShopifyAccess.Models.Order
{
	[ DataContract ]
	public class ShopifyBillingAddress
	{
		[ DataMember( Name = "address1" ) ]
		public string Address1 { get; set; }

		[ DataMember( Name = "address2" ) ]
		public string Address2 { get; set; }

		[ DataMember( Name = "city" ) ]
		public string City { get; set; }

		[ DataMember( Name = "company" ) ]
		public string Company { get; set; }

		[ DataMember( Name = "country" ) ]
		public string Country { get; set; }

		[ DataMember( Name = "province" ) ]
		public string Province { get; set; }

		[ DataMember( Name = "first_name" ) ]
		public string FirstName { get; set; }

		[ DataMember( Name = "last_name" ) ]
		public string LastName { get; set; }

		[ DataMember( Name = "phone" ) ]
		public string Phone { get; set; }

		[ DataMember( Name = "zip" ) ]
		public string Zip { get; set; }
	}
}