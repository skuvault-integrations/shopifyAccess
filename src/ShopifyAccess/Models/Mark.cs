using System;
using CuttingEdge.Conditions;

namespace ShopifyAccess.Models
{
	public class Mark
	{
		public string Value{ get; private set; }

		public Mark( string value )
		{
			Condition.Requires( value, "value" ).IsNotNullOrEmpty();

			this.Value = value;
		}

		public static Mark Create
		{
			get { return new Mark( GetTag() + Guid.NewGuid() ); }
		}

		public static string GetTag()
		{
			return "Mark-";
		}

		public override string ToString()
		{
			return this.Value;
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		#region Equality members
		public bool Equals( Mark other )
		{
			if( ReferenceEquals( null, other ) )
				return false;
			if( ReferenceEquals( this, other ) )
				return true;
			return string.Equals( this.Value, other.Value, StringComparison.InvariantCultureIgnoreCase );
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != this.GetType() )
				return false;
			return this.Equals( ( Mark )obj );
		}
		#endregion
	}

	public static class MarkExtensions
	{
		public static Mark CreateNewIfBlank( this Mark source )
		{
			return source ?? Mark.Create;
		}
	}
}