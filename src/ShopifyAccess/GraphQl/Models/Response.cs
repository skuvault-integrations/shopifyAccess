using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models
{
	[ DataContract ]
	public class Response< T > where T : class
	{
		[ DataMember( Name = "data" ) ]
		public T Data{ get; set; }

		//ToDo add "extensions" to get QueryCost and currentlyAvailable
	}
}