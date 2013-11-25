using System.Linq;
using CuttingEdge.Conditions;

namespace ShopifyAccess.Models.Configuration.Command
{
	internal class ShopifyCommandEndpointConfig
	{
		public int Page { get; private set; }
		public int Limit { get; private set; }
		public string Fields { get; private set; }

		public ShopifyCommandEndpointConfig( int page, int limit, params string[] fields )
			: this( limit, fields )
		{
			Condition.Requires( page, "page" ).IsGreaterThan( 0 );
			Condition.Requires( limit, "limit" ).IsGreaterThan( 0 );

			this.Page = page;
		}

		public ShopifyCommandEndpointConfig( int limit, params string[] fields )
		{
			Condition.Requires( limit, "limit" ).IsGreaterThan( 0 );

			this.Limit = limit;
			this.Fields = string.Join( ",", fields.ToArray() );
		}
	}
}