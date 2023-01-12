using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.BulkOperation
{
	[ DataContract ]
	public class BulkOperationRunQueryResponse
	{
		[ DataMember( Name = "data" ) ]
		public BulkOperationRunQueryData Data{ get; set; }
	}

	[ DataContract ]
	public class BulkOperationRunQueryData
	{
		[ DataMember( Name = "bulkOperationRunQuery" ) ]
		public BulkOperationRunQuery BulkOperationRunQuery{ get; set; }
	}

	[ DataContract ]
	public class BulkOperationRunQuery
	{
		[ DataMember( Name = "bulkOperation" ) ]
		public BulkOperation BulkOperation{ get; set; }
	}

	[ DataContract ]
	public class BulkOperation
	{
		[ DataMember( Name = "id" ) ]
		public string Id{ get; set; }

		[ DataMember( Name = "status" ) ]
		public string Status{ get; set; }
	}
}