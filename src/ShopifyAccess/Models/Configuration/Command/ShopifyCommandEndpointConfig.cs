using CuttingEdge.Conditions;

namespace ShopifyAccess.Models.Configuration.Command
{
	public class ShopifyCommandEndpointConfig
	{
		public long SinceId{ get; private set; }
		public int Limit{ get; private set; }

		public ShopifyCommandEndpointConfig( long sinceId, int limit )
			: this( limit )
		{
			Condition.Requires( sinceId, "page" ).IsGreaterOrEqual( 0 );
			Condition.Requires( limit, "limit" ).IsGreaterThan( 0 );

			this.SinceId = sinceId;
		}

		public ShopifyCommandEndpointConfig( int limit )
		{
			Condition.Requires( limit, "limit" ).IsGreaterThan( 0 );

			this.Limit = limit;
		}
	}
}