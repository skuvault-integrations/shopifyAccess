using System;
using System.Linq;

namespace ShopifyAccess.Models.Configuration.Authorization
{
	public class Scopes
	{
		public string ScopesString { get; private set; }

		public Scopes( params Scope[] scopes )
		{
			if( scopes == null || !scopes.Any() )
			{
				throw new ArgumentException( "scopes must not be empty", nameof(scopes) );
			}

			this.ScopesString = string.Join( ",", scopes.Select( s => s.Description ) );
		}
	}
}