using System.Collections.Generic;
using System.Runtime.Serialization;
using ShopifyAccess.GraphQl.Models.Pagination;

namespace ShopifyAccess.GraphQl.Models.ProductVariant
{
	[ DataContract ]
	public class ProductVariants
	{
		[ DataMember( Name = "nodes" ) ]
		public List< ProductVariant > Variants{ get; set; }

		[ DataMember( Name = "pageInfo" ) ]
		public PageInfo PageInfo{ get; set; }

		public ProductVariants()
		{
			this.Variants = new List< ProductVariant >();
		}
	}
}