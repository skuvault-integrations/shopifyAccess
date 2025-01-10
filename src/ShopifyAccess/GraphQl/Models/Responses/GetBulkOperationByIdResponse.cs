using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Responses
{
	[ DataContract ]
	internal class GetBulkOperationByIdResponse: BaseGraphQlResponse< BulkOperationByIdData >
	{
	}

	[ DataContract ]
	internal class BulkOperationByIdData
	{
		[ DataMember( Name = "node" ) ]
		public CurrentBulkOperation BulkOperation{ get; set; }
	}
}