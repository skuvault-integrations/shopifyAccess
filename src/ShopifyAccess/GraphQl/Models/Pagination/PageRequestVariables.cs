using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Pagination
{
	[ DataContract ]
	public class PageRequestVariables
	{
		[ DataMember( Name = "first" ) ]
		private int First{ get; set; }

		[ DataMember( Name = "after" ) ]
		private string After{ get; set; }

		public PageRequestVariables( int first, string after = null )
		{
			this.First = first;
			this.After = after;
		}
	}
}