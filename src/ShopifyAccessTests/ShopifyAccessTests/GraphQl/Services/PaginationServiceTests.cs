using System.Collections.Generic;
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
		
		[ Test ]
		public async Task GetAllPagesAsync_ShouldRequestMorePages_WhenResponseIndicatesThereAreMorePages()
		{
			// Arrange
			var shopName = _randomizer.GetString();
			var graphQlThrottler = new ShopifyGraphQlThrottler( shopName );
			var service = new PaginationService( graphQlThrottler, shopName );
			var getProductsResponse = new GetProductsResponse
			{
				Data = new GetProductsData
				{
					Products = new Nodes< Product >
					{
						Items = new List< Product > 
						{
							new Product
							{
								//TODO Return different ones on the 2 pages, to then assert
								Title = "Product1"
							}
						},
						PageInfo = new PageInfo
						{
							//TODO Somehow on the 1st call return a response with HasNextPage = true & nextCursor
							//   On the 2nd call return a response with HasNextPage = false
							//Currently, an infinite loop because there's always more pages
							HasNextPage = true,
							EndCursor = _randomizer.GetString()
						}
					}
				},
				Extensions = MockQuotaRemaining()
			};
			getProductsResponse.GetItemsAndPagingInfo();
			//TODO How to assert the number of calls and what cursor it received? Try NSubstitute
			async Task< GraphQlResponseWithPages< GetProductsData, Product > > MockSendRequestDelegate( string nextCursor ) => await Task.FromResult( getProductsResponse );

			// Act
			var result = await service.GetAllPagesAsync( MockSendRequestDelegate, Mark.Create, CancellationToken.None );

			// Assert
			Assert.Multiple( () => {
				//expected number of products from both pages
				Assert.That( result, Has.Length.EqualTo( 2 ) );
				//TODO Assert the product titles in the result
			} );
		}

		//TODO GetAllPagesAsync_ShouldNotRequestMorePages_WhenResponseIndicatesThereAreNoMorePages
		
		private static GraphQlExtensions MockQuotaRemaining()
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
}