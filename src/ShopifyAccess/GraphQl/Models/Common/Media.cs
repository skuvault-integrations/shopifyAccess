using System;
using System.Runtime.Serialization;

namespace ShopifyAccess.GraphQl.Models.Common
{
	internal class Media
	{
		[ DataMember( Name = "mediaContentType" ) ]
		public string MediaContentType{ get; set; }
		public MediaContentType MediaContentTypeStandardized => 
			Enum.TryParse( this.MediaContentType, true, out MediaContentType mediaContentType ) ? mediaContentType : Common.MediaContentType.Undefined;

		[ DataMember( Name = "preview" ) ]
		public MediaPreview Preview{ get; set; }
	}

	public enum MediaContentType
	{
		VIDEO,
		EXTERNAL_VIDEO,
		MODEL_3D,
		IMAGE,
		Undefined
	}
	
	public class MediaPreview
	{
		[ DataMember( Name = "image" ) ]
		public Image Image{ get; set; }
	}
}