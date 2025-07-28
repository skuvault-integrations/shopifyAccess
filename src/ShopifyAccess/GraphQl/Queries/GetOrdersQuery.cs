namespace ShopifyAccess.GraphQl.Queries
{
	public static class GetOrdersQuery
	{
// 		public static readonly string Query = @"query GetOrdersPaginated($query: String!, $first: Int!, $after: String) {
//   orders(first: $first, after: $after, query: $query) {
//     edges {
//       cursor
//       node {
//         id
//         name
//         createdAt
//         displayFinancialStatus
//         displayFulfillmentStatus
//       }
//     }
//     pageInfo {
//       hasNextPage
//       endCursor
//     }
//   }
// }";
	public static readonly string Query = @"
query {
  orders(first: 1) {
    edges {
      node {
        id
        name
        createdAt
      }
    }
    pageInfo {
      hasNextPage
      endCursor
    }
  }
}";
	}
}
