using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models
{
	[ DataContract ]
	internal class BaseGraphQlResponse< TData >
	{
		[ DataMember( Name = "data" ) ]
		public TData Data{ get; set; }

		[ DataMember( Name = "errors" ) ]
		public GraphQlError[] Errors{ get; set; }

		[ DataMember( Name = "extensions" ) ]
		public GraphQlExtensions Extensions{ get; set; }
	}
	
	[ DataContract ]
	internal abstract class BaseGraphQlResponseWithItems< TData, TItem > : BaseGraphQlResponse< TData >
	{
		public abstract List< TItem > GetItems();
	}

	[ DataContract ]
	internal class GraphQlError
	{
		[ DataMember( Name = "message" ) ]
		public string Message{ get; set; }

		[ DataMember( Name = "extensions" ) ]
		public GraphQlErrorExtensions Extensions{ get; set; }
	}

	[ DataContract ]
	internal class GraphQlErrorExtensions
	{
		[ DataMember( Name = "code" ) ]
		public string Code{ get; set; }
	}

	[ DataContract ]
	internal class GraphQlExtensions
	{
		[ DataMember( Name = "cost" ) ]
		public Cost Cost{ get; set; }
	}

	[ DataContract ]
	internal class Cost
	{
		[ DataMember( Name = "requestedQueryCost" ) ]
		public int RequestedQueryCost{ get; set; }

		[ DataMember( Name = "actualQueryCost" ) ]
		public int? ActualQueryCost{ get; set; }

		[ DataMember( Name = "throttleStatus" ) ]
		public ThrottleStatus ThrottleStatus{ get; set; }
	}

	[ DataContract ]
	internal class ThrottleStatus
	{
		[ DataMember( Name = "maximumAvailable" ) ]
		public float MaximumAvailable{ get; set; }

		[ DataMember( Name = "currentlyAvailable" ) ]
		public int CurrentlyAvailable{ get; set; }

		[ DataMember( Name = "restoreRate" ) ]
		public float RestoreRate{ get; set; }
	}
}