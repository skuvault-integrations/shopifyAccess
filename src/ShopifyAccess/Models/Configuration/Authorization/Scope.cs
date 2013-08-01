using CuttingEdge.Conditions;

namespace ShopifyAccess.Models.Configuration.Authorization
{
	public class Scope
	{
		public string Description { get; private set; }

		public Scope( ShopifyScopeAccessLevel scopeAccessLevel, ShopifyScopeName scopeName )
		{
			Condition.Requires( scopeAccessLevel, "scopeAccessLevel" ).IsGreaterThan( ShopifyScopeAccessLevel.Undefined );
			Condition.Requires( scopeName, "scopeName" ).IsNotNull();

			this.Description = string.Concat( scopeAccessLevel.ToString().ToLowerInvariant(), scopeName.Name );
		}
	}
}