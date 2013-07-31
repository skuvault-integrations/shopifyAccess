using CuttingEdge.Conditions;

namespace ShopifyAccess.Models.Configuration.Authorization
{
	public class Scope
	{
		public string Description { get; private set; }

		public Scope( ShopifyScopeAccessLevelEnum scopeAccessLevel, ShopifyScopeName scopeName )
		{
			Condition.Requires( scopeAccessLevel, "scopeAccessLevel" ).IsGreaterThan( ShopifyScopeAccessLevelEnum.Undefined );
			Condition.Requires( scopeName, "scopeName" ).IsNotNull();

			this.Description = string.Concat( scopeAccessLevel.ToString().ToLowerInvariant(), scopeName.Name );
		}
	}
}