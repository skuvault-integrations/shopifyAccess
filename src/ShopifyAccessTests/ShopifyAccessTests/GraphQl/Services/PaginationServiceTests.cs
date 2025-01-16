using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ShopifyAccess.GraphQl;
using ShopifyAccess.GraphQl.Models;
using ShopifyAccess.GraphQl.Models.Products;
using ShopifyAccess.GraphQl.Models.Responses;
using ShopifyAccess.GraphQl.Services;
using ShopifyAccess.Models;

namespace ShopifyAccessTests.GraphQl.Services
{
	public class PaginationServiceTests
	{
		private static readonly Randomizer _randomizer = new Randomizer();
		private PaginationService _service;
		
		[ SetUp ]
		public void Init()
		{
			var shopName = _randomizer.GetString();
			var graphQlThrottler = new ShopifyGraphQlThrottler( shopName );
			this._service = new PaginationService( graphQlThrottler, shopName );
		}
		
		[ Test ]
		public async Task GetAllPagesAsync_ShouldGetItemsFromAllPages_WhenResponseIndicatesThereAreMorePages()
		{
			// Arrange
			var product1Title = "product1";
			var cursorForSecondPage = _randomizer.GetString();
			var getProductsResponse1 = this.CreateGetProductsResponse( product1Title, hasNextPage: true, cursorForSecondPage );
			var product2Title = "product2";
			var getProductsResponse2 = this.CreateGetProductsResponse( product2Title, hasNextPage: false );
			var getProductsResponses = new List< GetProductsResponse > { getProductsResponse1, getProductsResponse2 };
			
			var sendRequestDelegateMocker = new GetProductsResponseMocker( getProductsResponses );
			
			// Act
			var result = await this._service.GetAllPagesAsync( sendRequestDelegateMocker.GetResponseAsync, Mark.Create, CancellationToken.None );

			// Assert
			Assert.Multiple( () =>
			{
				Assert.That( result, Has.Count.EqualTo( getProductsResponses.Count ) );
				Assert.That( result[ 0 ].Title, Is.EqualTo( product1Title ) );
				Assert.That( result[ 1 ].Title, Is.EqualTo( product2Title ) );
				Assert.That( sendRequestDelegateMocker.EndCursorsReceived.Count, Is.EqualTo( getProductsResponses.Count ) );
				Assert.That( sendRequestDelegateMocker.EndCursorsReceived[ 1 ], Is.EqualTo( cursorForSecondPage ) );
			} );
		}

		//TODO GUARD-3717 GetAllPagesAsync_ShouldNotRequestMorePages_WhenResponseIndicatesThereAreNoMorePages
		
		private GetProductsResponse CreateGetProductsResponse( string productTitle, bool hasNextPage = false, string endCursor = null)
		{
			return new GetProductsResponse
			{
				Data = new GetProductsData
				{
					Products = new Nodes< Product >
					{
						Items = new List< Product > 
						{
							new Product
							{
								Title = productTitle
							}
						},
						PageInfo = new PageInfo
						{
							HasNextPage = hasNextPage,
							EndCursor = endCursor ?? _randomizer.GetString()
						}
					}
				},
				Extensions = GetNonThrottledResponseExtensions()
			};
		}
		
		private static GraphQlExtensions GetNonThrottledResponseExtensions()
		{
			var requestedQueryCost = _randomizer.Next( 1, 10 );
			return new GraphQlExtensions
			{
				Cost = new Cost
				{
					ThrottleStatus = new ThrottleStatus
					{
						//Ensure more is available, so that the throttler isn't triggered
						CurrentlyAvailable = requestedQueryCost + 1000,
						RestoreRate = _randomizer.Next( 1, 10 )
					},
					RequestedQueryCost = requestedQueryCost
				}
			};
		}
	}
	
	/// <summary>
	/// Mocks sendRequestAsync responses based on the passed-in getProductsResponses
	/// </summary>
	internal class GetProductsResponseMocker
	{
		internal List< string > EndCursorsReceived { get; } = new List< string >();
		private readonly GetProductsResponse [] _getProductsResponses;

		public GetProductsResponseMocker( IEnumerable< GetProductsResponse > getProductsResponses )
		{
			this._getProductsResponses = getProductsResponses.ToArray();
		}
		
		internal async Task< GraphQlResponseWithPages< GetProductsData, Product > > GetResponseAsync( string endCursor )
		{
			if ( this.EndCursorsReceived.Count > this._getProductsResponses.Length )
			{
				return null;
			}
			this.EndCursorsReceived.Add( endCursor );
			return await Task.FromResult( this._getProductsResponses[ this.EndCursorsReceived.Count - 1 ] );
		}
	}
}