using CuttingEdge.Conditions;

namespace ShopifyAccess.Models.Configuration.Command
{
	internal class ShopifyCommandEndpointConfig
	{
		public int Page { get; private set; }
		public int Limit { get; private set; }

		public ShopifyCommandEndpointConfig( int page, int limit )
			: this( limit )
		{
			Condition.Requires( page, "page" ).IsGreaterThan( 0 );
			Condition.Requires( limit, "limit" ).IsGreaterThan( 0 );

			this.Page = page;
		}

		public ShopifyCommandEndpointConfig( int limit )
		{
			Condition.Requires( limit, "limit" ).IsGreaterThan( 0 );

			this.Limit = limit;
		}
	}
}