using System.Collections.Generic;
using System.Linq;
using ShopifyAccess.Misc;
using ShopifyAccess.Models;

namespace ShopifyAccess.GraphQl.Models.Common.Extensions
{
	internal static class MediaExtensions
	{
		/// <summary>
		/// Get media items of type image
		/// </summary>
		/// <param name="mediaItems"></param>
		/// <returns></returns>
		internal static List< ShopifyProductImage > ToShopifyProductImages( this List< Media > mediaItems )
		{
			return mediaItems?.Where( x => x.MediaContentTypeStandardized == MediaContentType.IMAGE && !string.IsNullOrWhiteSpace( x.Preview?.Image?.Url ) )?
					.Select( x => new ShopifyProductImage
						{ Src = x.Preview.Image.Url.GetUrlWithoutQueryPart() } ).ToList()
				?? new List< ShopifyProductImage >();
		}
	}
}