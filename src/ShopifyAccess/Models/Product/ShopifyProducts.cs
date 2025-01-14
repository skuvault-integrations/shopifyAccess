﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using ShopifyAccess.Models.ProductVariant;

namespace ShopifyAccess.Models.Product
{
	//TODO GUARD-3717 [Cleanup] Remove all [ Data* ] attributes since this will not be directly deserialized from GraphQL
	[ DataContract ]
	public class ShopifyProducts
	{
		[ DataMember( Name = "products" ) ]
		public List< ShopifyProduct > Products { get; set; }

		public ShopifyProducts()
		{
			this.Products = new List< ShopifyProduct >();
		}
	}

	public static class ShopifyProductsExtensions
	{
		//TODO GUARD-3717 [Cleanup] Remove if not used in v1
		public static IDictionary< string, ShopifyProductVariant > ToDictionary( this ShopifyProducts shopifyInventory )
		{
			var inventory = new Dictionary< string, ShopifyProductVariant >();
			foreach( var product in shopifyInventory.Products )
			{
				if( product.Variants == null )
					continue;

				foreach( var variant in product.Variants )
				{
					if( variant == null || variant.Sku == null )
						continue;

					if( !inventory.ContainsKey( variant.Sku ) )
						inventory.Add( variant.Sku, variant );
				}
			}
			return inventory;
		}
	}
}