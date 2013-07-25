using System.Linq;
using CuttingEdge.Conditions;

namespace ShopifyAccess.Models.Core.Configuration.Authorization
{
	public class Scopes
	{
		public string ScopesString { get; private set; }

		public Scopes( params Scope[] scopes )
		{
			Condition.Requires( scopes, "scopes" ).IsNotEmpty();

			this.ScopesString = string.Join( ",", scopes.Select( s => s.Description ) );
		}
	}
}