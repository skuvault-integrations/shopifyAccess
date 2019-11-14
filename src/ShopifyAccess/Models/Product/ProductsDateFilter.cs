using System;

namespace ShopifyAccess.Models.Product
{
	public struct ProductsDateFilter
	{
		public FilterType FilterType;
		public DateTime ProductsStartUtc;
	}

	public enum FilterType
	{
		None,
		CreatedAfter,
		CreatedBeforeUpdatedAfter
	};
}
