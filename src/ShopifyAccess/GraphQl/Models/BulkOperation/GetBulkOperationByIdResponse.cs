using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.BulkOperation
{
	[ DataContract ]
	public class GetBulkOperationByIdResponse
	{
		[ DataMember( Name = "data" ) ]
		public BulkOperationByIdData Data{ get; set; }
	}

	[ DataContract ]
	public class BulkOperationByIdData
	{
		[ DataMember( Name = "node" ) ]
		public CurrentBulkOperation BulkOperation{ get; set; }
	}
}