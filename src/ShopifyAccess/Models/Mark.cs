using System;

namespace ShopifyAccess.Models
{
	public class Mark
	{
		public string Value{ get; private set; }
		public static Mark Create{ get; } = new Mark( Guid.NewGuid().ToString() );

		public static Mark Blank()
		{
			return new Mark( string.Empty );
		}

		private static Mark CreateNew()
		{
			return new Mark( Guid.NewGuid().ToString() );
		}

		public Mark( string value )
		{
			this.Value = value;
		}

		public override string ToString()
		{
			return GetTag() + this.Value;
		}

		public static string GetTag()
		{
			return "Mark-";
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
		public static bool IsBlank( this Mark source )
		{
			return string.IsNullOrWhiteSpace( source?.Value );
		}

		public static Mark CreateNewIfBlank( this Mark source )
		{
			return string.IsNullOrWhiteSpace( source?.Value ) ? Mark.Create : source;
		}

		public static string ToJsonObjectSafe( this Mark source )
		{
			return IsBlank( source ) ? PredefinedValues.EmptyJsonObject : source.ToString();
		}

		public static string ToStringSafe( this Mark source )
		{
			return IsBlank( source ) ? Mark.GetTag() : source.ToString();
		}
	}

	public static class PredefinedValues
	{
		public static string EmptyJsonObject{ get; } = "{}";
	}
}