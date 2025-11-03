using System;

namespace ShopifyAccess.Models.Configuration.Command
{
	public class ShopifyCommandEndpointConfig
	{
		public int Limit{ get; private set; }

		public ShopifyCommandEndpointConfig( int limit )
		{
			if( limit <= 0 )
			{
				throw new ArgumentOutOfRangeException( nameof(limit), limit, "limit must be greater than 0" );
			}


			this.Limit = limit;
		}
	}
}