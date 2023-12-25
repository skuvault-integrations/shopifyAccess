using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using ShopifyAccess.Exceptions;
using ShopifyAccess.Models;
using ShopifyAccess.Models.Configuration.Command;
using ShopifyAccess.Services;

namespace ShopifyAccessTests.Services
{
	[ TestFixture ]
	public class WebRequestServicesTests: BaseTests
	{
		private readonly ShopifyCommand _TestCommand = new ShopifyCommand( "/test/command.json", ApiVersion );
		private const int Timeout = 10000;
		private const string responseOkJson = "\"ok\"";

		#region GetResponse
		[ Test ]
		[ TestCase( HttpStatusCode.InternalServerError ) ]
		[ TestCase( HttpStatusCode.NotImplemented ) ]
		[ TestCase( HttpStatusCode.BadGateway ) ]
		[ TestCase( HttpStatusCode.ServiceUnavailable ) ]
		[ TestCase( HttpStatusCode.GatewayTimeout ) ]
		public void GetResponse_ThrowsShopifyTransientException_When5xxStatusCode( HttpStatusCode statusCode )
		{
			// Arrange
			var client = GetHttpClient( statusCode, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			var action = new Action( () => webRequestServices.GetResponse< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout ) );

			// Assert
			action.Should().Throw< ShopifyTransientException >();
		}

		[ Test ]
		public void GetResponse_ThrowsShopifyUnauthorizedException_WhenWrongCredentials()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.Unauthorized, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			var action = new Action( () => webRequestServices.GetResponse< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout ) );

			// Assert
			action.Should().Throw< ShopifyUnauthorizedException >();
		}

		[ Test ]
		public void GetResponse_ThrowsHttpRequestException_WhenNotFound()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.NotFound, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			var action = new Action( () => webRequestServices.GetResponse< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout ) );

			// Assert
			action.Should().Throw< HttpRequestException >();
		}

		[ Test ]
		public void GetResponse_ReturnsResponse_WhenCorrectCommand()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.OK, new StringContent( responseOkJson ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			var response = webRequestServices.GetResponse< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout );

			// Assert
			AssertExpectedResponse( response, responseOkJson );
		}

		#endregion

		#region GetResponseAsync
		[ Test ]
		public void GetResponseAsync_ThrowsShopifyUnauthorizedException_WhenWrongCredentials()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.Unauthorized, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			Func< Task > action = async () => await webRequestServices.GetResponseAsync< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout );

			// Assert
			action.Should().ThrowAsync< ShopifyUnauthorizedException >();
		}

		[ Test ]
		public void GetResponseAsync_ThrowsHttpRequestException_WhenNotFound()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.NotFound, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			Func< Task > action = async () => await webRequestServices.GetResponseAsync< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout );

			// Assert
			action.Should().ThrowAsync< HttpRequestException >();
		}

		[ Test ]
		public async Task GetResponseAsync_ReturnsResponse_WhenCorrectCommand()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.OK, new StringContent( responseOkJson ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			var response = await webRequestServices.GetResponseAsync< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout );

			// Assert
			AssertExpectedResponse( response, responseOkJson );
		}
		#endregion

		#region GetResponsePage
		[ Test ]
		public void GetResponsePage_ThrowsShopifyUnauthorizedException_WhenWrongCredentials()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.Unauthorized, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			var action = new Action( () => webRequestServices.GetResponsePage< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout ) );

			// Assert
			action.Should().Throw< ShopifyUnauthorizedException >();
		}

		[ Test ]
		public void GetResponsePage_ThrowsHttpRequestException_WhenNotFound()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.NotFound, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			var action = new Action( () => webRequestServices.GetResponsePage< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout ) );

			// Assert
			action.Should().Throw< HttpRequestException >();
		}

		[ Test ]
		public void GetResponsePage_ReturnsResponse_WhenCorrectCommand()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.OK, new StringContent( responseOkJson ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			var response = webRequestServices.GetResponsePage< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout );

			// Assert
			AssertExpectedResponse( response.Response, responseOkJson );
		}
		#endregion

		#region GetResponsePageAsync
		[ Test ]
		public void GetResponsePageAsync_ThrowsShopifyUnauthorizedException_WhenWrongCredentials()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.Unauthorized, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			Func< Task > action = async () => await webRequestServices.GetResponsePageAsync< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout );

			// Assert
			action.Should().ThrowAsync< ShopifyUnauthorizedException >();
		}

		[ Test ]
		public void GetResponsePageAsync_ThrowsHttpRequestException_WhenNotFound()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.NotFound, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			Func< Task > action = async () => await webRequestServices.GetResponsePageAsync< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout );

			// Assert
			action.Should().ThrowAsync< HttpRequestException >();
		}

		[ Test ]
		public async Task GetResponsePageAsync_ReturnsResponse_WhenCorrectCommand()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.OK, new StringContent( responseOkJson ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			var response = await webRequestServices.GetResponsePageAsync< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout );

			// Assert
			AssertExpectedResponse( response.Response, responseOkJson );
		}
		#endregion

		#region PostData
		[ Test ]
		public void PostData_ThrowsShopifyUnauthorizedException_WhenWrongCredentials()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.Unauthorized, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			var action = new Action( () => webRequestServices.PostData< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout ) );

			// Assert
			action.Should().Throw< ShopifyUnauthorizedException >();
		}

		[ Test ]
		public void PostData_ThrowsHttpRequestException_WhenNotFound()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.NotFound, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			var action = new Action( () => webRequestServices.PostData< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout ) );

			// Assert
			action.Should().Throw< HttpRequestException >();
		}

		[ Test ]
		public void PostData_ReturnsResponse_WhenCorrectCommand()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.OK, new StringContent( responseOkJson ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			var response = webRequestServices.PostData< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout );

			// Assert
			AssertExpectedResponse( response, responseOkJson );
		}
		#endregion

		#region PostDataAsync
		[ Test ]
		public void PostDataAsync_ThrowsShopifyUnauthorizedException_WhenWrongCredentials()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.Unauthorized, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			Func< Task > action = async () => await webRequestServices.PostDataAsync< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout );

			// Assert
			action.Should().ThrowAsync< ShopifyUnauthorizedException >();
		}

		[ Test ]
		public void PostDataAsync_ThrowsHttpRequestException_WhenNotFound()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.NotFound, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			Func< Task > action = async () => await webRequestServices.PostDataAsync< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout );

			// Assert
			action.Should().ThrowAsync< HttpRequestException >();
		}

		[ Test ]
		public async Task PostDataAsync_ReturnsResponse_WhenCorrectCommand()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.OK, new StringContent( responseOkJson ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			var response = await webRequestServices.PostDataAsync< string >( this._TestCommand, "", CancellationToken.None, Mark.Create, Timeout );

			// Assert
			AssertExpectedResponse( response, responseOkJson );
		}
		#endregion

		#region GetReportDocumentAsync
		[ Test ]
		public void GetReportDocumentAsync_ThrowsShopifyUnauthorizedException_WhenWrongCredentials()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.Unauthorized, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			Func< Task > action = async () => await webRequestServices.GetReportDocumentAsync( "http://test.com", this.ParseFunc, CancellationToken.None, Mark.Create, Timeout );

			// Assert
			action.Should().ThrowAsync< ShopifyUnauthorizedException >();
		}

		[ Test ]
		public void GetReportDocumentAsync_ThrowsHttpRequestException_WhenNotFound()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.NotFound, new StringContent( string.Empty ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			Func< Task > action = async () => await webRequestServices.GetReportDocumentAsync( "http://test.com", this.ParseFunc, CancellationToken.None, Mark.Create, Timeout );

			// Assert
			action.Should().ThrowAsync< HttpRequestException >();
		}

		[ Test ]
		public async Task GetReportDocumentAsync_ReturnsResponse_WhenCorrectCommand()
		{
			// Arrange
			var client = GetHttpClient( HttpStatusCode.OK, new StringContent( responseOkJson ) );
			var webRequestServices = new WebRequestServices( new ShopifyClientCredentials( "shopName", "accessToken" ), client );

			// Act
			var response = await webRequestServices.GetReportDocumentAsync( "http://test.com", this.ParseFunc, CancellationToken.None, Mark.Create, Timeout );

			// Assert
			response.Should().BeEquivalentTo( new[] { responseOkJson } );
		}
		#endregion

		private IEnumerable< string > ParseFunc( Stream stream )
		{
			var result = new List< string >( 1 );
			using( var reader = new StreamReader( stream ) )
			{
				string line;
				while( ( line = reader.ReadLine() ) != null )
					result.Add( line );
			}

			return result;
		}

		private static HttpClient GetHttpClient( HttpStatusCode statusCode, HttpContent content )
		{
			var httpMessageHandlerMock = Substitute.For< HttpMessageHandler >();
			httpMessageHandlerMock.GetType().GetMethod(
					"SendAsync",
					BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly,
					null,
					new[] { typeof( HttpRequestMessage ), typeof( CancellationToken ) },
					null )
				?.Invoke( httpMessageHandlerMock, new object[]
				{
					Arg.Any< HttpRequestMessage >(),
					Arg.Any< CancellationToken >()
				} )
				.Returns( Task.FromResult( new HttpResponseMessage
				{
					StatusCode = statusCode,
					Content = content,
				} ) );

			return new HttpClient( httpMessageHandlerMock );
		}
		
		private void AssertExpectedResponse( string response, string expectedResponseJson )
		{
			var expectedResponse = expectedResponseJson.Replace( "\"", string.Empty );
			response.Should().Be( expectedResponse );
		}
	}
}