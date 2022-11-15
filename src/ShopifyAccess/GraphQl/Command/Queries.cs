namespace ShopifyAccess.GraphQl
{
	internal static class Queries
	{
		internal const string GetProductVariantsQuery = @"
query GetProductVariants($first: Int! $after: String){
 productVariants(first: $first, after: $after) {
  nodes {
   id
   sku
  }
  pageInfo {
      endCursor
      hasNextPage
  }
 }
}";
	}
}