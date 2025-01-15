using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ShopifyAccess.GraphQl.Models.Common;
using ShopifyAccess.GraphQl.Models.Common.Extensions;

namespace ShopifyAccessTests.GraphQl.Models.Common.Extensions
{
	public class MediaExtensionsTests
	{
		[ TestCase( "VIDEO" ) ]
		[ TestCase( "" ) ]
		[ TestCase( "BOB" ) ]
		public void ToShopifyProductImages_ReturnsEmptyList_WhenMediaItemTypeIsNotImage( string nonImageMediaContentType )
		{
			var mediaItems = new List< Media > { new Media { MediaContentType = nonImageMediaContentType } };

			var result = mediaItems.ToShopifyProductImages();

			Assert.That( result, Is.Empty );
		}

		[ Test ]
		public void ToShopifyProductImages_ReturnsEmptyList_WhenMediaItemsListIsNull()
		{
			List< Media > mediaItems = null;

			var result = mediaItems.ToShopifyProductImages();

			Assert.That( result, Is.Empty );
		}
		
		[ Test ]
		public void ToShopifyProductImages_ReturnsEmptyList_WhenMediaItemPreviewImageIsNull()
		{
			var mediaItems = new List< Media > { new Media { MediaContentType = MediaContentType.IMAGE.ToString(), Preview = null } };

			var result = mediaItems.ToShopifyProductImages();

			Assert.That( result, Is.Empty );
		}

		[ TestCase( " ") ]
		[ TestCase( "") ]
		[ TestCase( null) ]
		public void ToShopifyProductImages_ReturnsEmptyList_WhenMediaItemPreviewImageUrlIsBlank( string blankUrl )
		{
			var mediaItems = new List< Media > { new Media { MediaContentType = MediaContentType.IMAGE.ToString(), Preview = new MediaPreview { Image = new Image { Url = blankUrl } } } };

			var result = mediaItems.ToShopifyProductImages();

			Assert.That( result, Is.Empty );
		}

		[ Test ]
		public void ToShopifyProductImages_ReturnsOneItem_WhenMediaTypeIsImage_andPreviewImageUrlIsNotBlank()
		{
			var mediaItems = new List< Media > { new Media { MediaContentType = MediaContentType.IMAGE.ToString(), Preview = new MediaPreview { Image = new Image { Url = "http://test.com/image.jpg" } } } };

			var result = mediaItems.ToShopifyProductImages();

			Assert.That( result.Single().Src, Is.EqualTo( "http://test.com/image.jpg" ) );
		}

		[ Test ]
		public void ToShopifyProductImages_ReturnsOneItemWithQueryStringRemoved_WhenMediaTypeIsImage_andPreviewImageUrlIsNotBlank()
		{
			const string previewImageUrlWithoutQuery = "http://test.com/image.jpg";
			var mediaItems = new List< Media > { new Media { MediaContentType = MediaContentType.IMAGE.ToString(),
				Preview = new MediaPreview { Image = new Image { Url = $"{previewImageUrlWithoutQuery}?query=2&something" } } } };

			var result = mediaItems.ToShopifyProductImages();

			Assert.That( result.Single().Src, Is.EqualTo( previewImageUrlWithoutQuery ) );
			
		}
	}
}