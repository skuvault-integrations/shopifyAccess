using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Common
{
	public class Image
	{
		[ DataMember( Name = "url" ) ]
		public string Url{ get; set; }
	}
}