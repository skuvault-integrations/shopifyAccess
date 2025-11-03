using System;

namespace ShopifyAccess.Models.Configuration.Authorization
{
	public class Scope
	{
		public string Description { get; private set; }

		public Scope( ShopifyScopeAccessLevel scopeAccessLevel, ShopifyScopeName scopeName )
		{
			if( scopeAccessLevel <= ShopifyScopeAccessLevel.Undefined )
			{
				throw new ArgumentOutOfRangeException( nameof(scopeAccessLevel), scopeAccessLevel, "scopeAccessLevel must be greater than ShopifyScopeAccessLevel.Undefined" );
			}

			if( scopeName == null )
			{
				throw new ArgumentNullException( nameof(scopeName), "scopeName must not be null" );
			}


			this.Description = string.Concat( scopeAccessLevel.ToString().ToLowerInvariant(), scopeName.Name );
		}
	}
}