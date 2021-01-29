using System.Net.Http.Headers;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.Services;

namespace ShopifyAccessTests
{
	[ TestFixture ]
	public class WebRequestServicesTests
	{
		[ Test ]
		public void GetNextPageLinkFromHeader_IfFirstPage_ShouldReturnNextPage()
		{
			const string queryStr = "?limit=50&page_info=eyJkaXJlY3";
			var responseHeaders = new HttpTestHeaders( "Link", 
				"<https://skuvault.myshopify.com/admin/api/2019-10/products.json" + queryStr + ">; rel=\"next\"" );

			var link = PagedResponseService.GetNextPageQueryStrFromHeader( responseHeaders );

			link.Should().Be( queryStr );
		}

		[ Test ]
		public void GetNextPageLinkFromHeader_IfBothPrevAndNextPagesPresent_ShouldReturnNextPage()
		{
			const string queryStr = "?limit=50&page_info=eyJkaXJlY3";
			var responseHeaders = new HttpTestHeaders( "Link", 
				"<https://skuvault.myshopify.com/admin/api/2019-10/products.json?limit=50&page_info=eyJkaXJlY3>; rel=\"previous\", " + 
				"<https://skuvault.myshopify.com/admin/api/2019-10/products.json" + queryStr + ">; rel=\"next\"" );

			var link = PagedResponseService.GetNextPageQueryStrFromHeader( responseHeaders );

			link.Should().Be( queryStr );
		}

		[ Test ]
		public void GetNextPageLinkFromHeader_IfLastPage_ShouldReturnNothing()
		{
			var responseHeaders = new HttpTestHeaders( "Link", 
				"<https://skuvault.myshopify.com/admin/api/2019-10/products.json?limit=50&page_info=eyJkaXJlY3>; rel=\"previous\"" );

			var link = PagedResponseService.GetNextPageQueryStrFromHeader( responseHeaders );

			link.Should().Be( "" );
		}

		private sealed class HttpTestHeaders : HttpHeaders 
		{
			public HttpTestHeaders( string key, string value )
			{
				base.Add( key, value );
			}
		}
	}
}
