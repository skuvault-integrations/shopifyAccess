using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Responses
{
	[ DataContract ]
	internal class BulkOperationRunQueryResponse: BaseGraphQlResponse
	{
		[ DataMember( Name = "data" ) ]
		public BulkOperationRunQueryData Data{ get; set; }
	}

	[ DataContract ]
	internal class BulkOperationRunQueryData
	{
		[ DataMember( Name = "bulkOperationRunQuery" ) ]
		public BulkOperationRunQuery BulkOperationRunQuery{ get; set; }
	}

	[ DataContract ]
	internal class BulkOperationRunQuery
	{
		[ DataMember( Name = "bulkOperation" ) ]
		public BulkOperation BulkOperation{ get; set; }
	}

	[ DataContract ]
	internal class BulkOperation
	{
		[ DataMember( Name = "id" ) ]
		public string Id{ get; set; }

		[ DataMember( Name = "status" ) ]
		public string Status{ get; set; }
	}
}