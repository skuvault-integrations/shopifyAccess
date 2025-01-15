using System;

namespace ShopifyAccess.Models
{
	public class ShopifyProductImage: IEquatable< ShopifyProductImage >
	{
		public string Src { get; set; }

		public bool Equals( ShopifyProductImage other )
		{
			return false;
		}

		public override bool Equals( object obj )
		{
			if( ReferenceEquals( null, obj ) )
				return false;
			if( ReferenceEquals( this, obj ) )
				return true;
			if( obj.GetType() != this.GetType() )
				return false;
			return Equals( ( ShopifyProductImage )obj );
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = this.Src.GetHashCode();

				return hashCode;
			}
		}
	}
}
