using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Responses
{
	/// <summary>
	/// While the operation is running, we can poll to see its progress using the currentBulkOperation command
	/// </summary>
	[ DataContract ]
	internal class GetCurrentBulkOperationResponse: BaseGraphQlResponse
	{
		[ DataMember( Name = "data" ) ]
		public CurrentBulkOperationData Data{ get; set; }
	}

	[ DataContract ]
	internal class CurrentBulkOperationData
	{
		[ DataMember( Name = "currentBulkOperation" ) ]
		public CurrentBulkOperation CurrentBulkOperation{ get; set; }
	}

	[ DataContract ]
	internal class CurrentBulkOperation
	{
		[ DataMember( Name = "id" ) ]
		public string Id{ get; set; }

		[ DataMember( Name = "status" ) ]
		public string Status{ get; set; }

		[ DataMember( Name = "errorCode" ) ]
		public string ErrorCode{ get; set; }

		[ DataMember( Name = "url" ) ]
		public string Url{ get; set; }

		[ DataMember( Name = "objectCount" ) ]
		public string ObjectCount{ get; set; }

		[ DataMember( Name = "fileSize" ) ]
		public string FileSize{ get; set; }
	}
}