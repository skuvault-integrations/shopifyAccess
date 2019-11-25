using CuttingEdge.Conditions;

namespace ShopifyAccess.Models.Configuration.Command
{
	public class ShopifyCommandEndpointConfig
	{
		public int Limit{ get; private set; }

		public ShopifyCommandEndpointConfig( int limit )
		{
			Condition.Requires( limit, "limit" ).IsGreaterThan( 0 );

			this.Limit = limit;
		}
	}
}