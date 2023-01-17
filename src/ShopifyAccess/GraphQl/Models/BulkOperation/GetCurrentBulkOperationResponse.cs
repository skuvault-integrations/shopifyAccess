using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.BulkOperation
{
	[ DataContract ]
	public class GetCurrentBulkOperationResponse
	{
		[ DataMember( Name = "data" ) ]
		public CurrentBulkOperationData Data{ get; set; }
	}

	[ DataContract ]
	public class CurrentBulkOperationData
	{
		[ DataMember( Name = "currentBulkOperation" ) ]
		public CurrentBulkOperation CurrentBulkOperation{ get; set; }
	}

	[ DataContract ]
	public class CurrentBulkOperation
	{
		[ DataMember( Name = "id" ) ]
		public string Id{ get; set; }

		[ DataMember( Name = "status" ) ]
		public string Status{ get; set; }

		[ DataMember( Name = "errorCode" ) ]
		public string ErrorCode{ get; set; }

		[ DataMember( Name = "url" ) ]
		public string Url{ get; set; }
	}
}