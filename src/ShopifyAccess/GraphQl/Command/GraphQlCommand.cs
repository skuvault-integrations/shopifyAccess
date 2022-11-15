using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl
{
	[ DataContract ]
	internal class GraphQlCommand
	{
		[ DataMember( Name = "query" ) ]
		private string Query{ get; set; }

		[ DataMember( Name = "variables" ) ]
		private object Variables{ get; set; }

		public GraphQlCommand( string query, object variables )
		{
			this.Query = query;
			this.Variables = variables;
		}
	}
}