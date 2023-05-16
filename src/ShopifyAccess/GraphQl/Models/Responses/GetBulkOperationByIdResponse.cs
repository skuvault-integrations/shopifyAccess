using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Responses
{
	[ DataContract ]
	internal class GetBulkOperationByIdResponse: BaseGraphQlResponse
	{
		[ DataMember( Name = "data" ) ]
		public BulkOperationByIdData Data{ get; set; }
	}

	[ DataContract ]
	internal class BulkOperationByIdData
	{
		[ DataMember( Name = "node" ) ]
		public CurrentBulkOperation BulkOperation{ get; set; }
	}
}