using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.ProductVariantsInventory
{
	[ DataContract ]
	internal class ProductVariants
	{
		[ DataMember( Name = "nodes" ) ]
		public List< ProductVariant > Nodes{ get; set; }

		[ DataMember( Name = "pageInfo" ) ]
		public PageInfo PageInfo{ get; set; }
	}
}