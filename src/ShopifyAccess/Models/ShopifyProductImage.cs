using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ShopifyAccess.Models
{
	[ DataContract ]
	//TODO GUARD-3717 [Cleanup] Remove all [ Data* ] attributes since this will not be directly deserialized from GraphQL
	public class ShopifyProductImage: IEquatable< ShopifyProductImage >
	{
		[ DataMember( Name = "id") ]
		[ Obsolete( "TODO GUARD-3717 [Cleanup] Remove" ) ]
		public long Id { get; set; }

		[ DataMember( Name = "src") ]
		public string Src { get; set; }

		[ DataMember( Name = "variant_ids" ) ]
		[ Obsolete( "TODO GUARD-3717 [Cleanup] Remove" ) ]
		public List< long > VariantIds { get; set; }

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
				var hashCode = this.Id.GetHashCode();
				hashCode = ( hashCode * 397 ) ^ this.Src.GetHashCode();

				return hashCode;
			}
		}
	}
}
