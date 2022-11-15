using ServiceStack;
using ShopifyAccess.GraphQl.Models;
using ShopifyAccess.GraphQl.Models.Pagination;

namespace ShopifyAccess.GraphQl
{
	public static class GraphQlRequestFactory
	{
		/// <summary>
		/// Returns GetProductVariants query
		/// </summary>
		/// <param name="first">Number of records in response</param>
		/// <param name="after">Start cursor</param>
		/// <returns></returns>
		public static string GetProductVariantsPage( int first, string after = null )
		{
			var variables = new PageRequestVariables( first, after );
			var command = new GraphQlCommand( Queries.GetProductVariantsQuery, variables );
			return command.ToJson();
		}
	}
}