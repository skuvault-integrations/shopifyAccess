using System;
using System.Threading.Tasks;
using FluentAssertions;
using Netco.ThrottlerServices;
using NUnit.Framework;
using ShopifyAccess.GraphQl;
using ShopifyAccess.GraphQl.Models;
using ShopifyAccess.GraphQl.Models.Products;
using ShopifyAccess.GraphQl.Models.Responses;
using ShopifyAccess.Models;
using ProductVariant = ShopifyAccess.GraphQl.Models.ProductVariantsInventory.ProductVariant;

namespace ShopifyAccessTests.GraphQl
{
	[ TestFixture ]
	public class ShopifyGraphQlThrottlerTests
	{
		private const int MaxRetryCount = 2;
		private readonly ShopifyGraphQlThrottler _throttler = new ShopifyGraphQlThrottler( "ShopName", MaxRetryCount );

		[ Test ]
		public async Task ExecuteAsync_ExecuteFunctionOneTime_WhenFunctionSuccess()
		{
			// Arrange
			var funcToThrottleCallCount = 0;
			var funcToThrottle = new Func< Task< BaseGraphQlResponse< ProductVariant > > >( () =>
			{
				funcToThrottleCallCount++;
				return Task.FromResult( this.GetBaseGraphQlResponse() );
			} );

			// Act
			await this._throttler.ExecuteAsync( funcToThrottle, Mark.Create );

			// Assert
			funcToThrottleCallCount.Should().Be( 1 );
		}

		[ Test ]
		public async Task ExecuteAsync_RetryMaxRetryCountAndThrowsThrottlerException_WhenFunctionFailedWithThrottledCode()
		{
			// Arrange
			var funcToThrottleCallCount = 0;
			var funcToThrottle = new Func< Task< BaseGraphQlResponse< ProductVariant > > >( () =>
			{
				funcToThrottleCallCount++;
				return Task.FromResult( this.GetThrottledErrorResponse() );
			} );

			// Act, Assert
			await this._throttler.Invoking( p => p.ExecuteAsync( funcToThrottle, Mark.Create ) ).Should().ThrowAsync< ThrottlerException >();
			funcToThrottleCallCount.Should().Be( MaxRetryCount );
		}

		[ Test ]
		public async Task ExecuteAsync_ThrowsSystemException_WhenFunctionFailedWithUnknownError()
		{
			// Arrange
			var funcToThrottleCallCount = 0;
			var funcToThrottle = new Func< Task< BaseGraphQlResponse< ProductVariant > > >( () =>
			{
				funcToThrottleCallCount++;
				return Task.FromResult( this.GetUnknownErrorResponse() );
			} );

			// Act, Assert
			await this._throttler.Invoking( p => p.ExecuteAsync( funcToThrottle, Mark.Create ) ).Should().ThrowAsync< SystemException >();
			funcToThrottleCallCount.Should().Be( 1 );
		}

		[ Test ]
		public async Task ExecuteAsync_RetryAndReturnsResult_WhenFunctionFailedFirstTimeWithThrottledCode_AndSuccessSecondTime()
		{
			// Arrange
			var funcToThrottleCallCount = 0;
			var funcToThrottle = new Func< Task< BaseGraphQlResponse< ProductVariant > > >( () =>
			{
				funcToThrottleCallCount++;
				return Task.FromResult( funcToThrottleCallCount == 0 ? this.GetThrottledErrorResponse() : this.GetBaseGraphQlResponse() );
			} );

			// Act
			await this._throttler.ExecuteAsync( funcToThrottle, Mark.Create );

			// Assert
			funcToThrottleCallCount.Should().Be( 1 );
		}

		[ Test ]
		public async Task ExecuteWithPaginationAsync_ExecuteFunctionOneTime_WhenFunctionSuccess()
		{
			// Arrange
			var funcToThrottleCallCount = 0;
			var funcToThrottle = new Func< Task< GraphQlResponseWithPages< GetProductsData, Product > > >( () =>
			{
				funcToThrottleCallCount++;
				return Task.FromResult( GetPaginatedGraphQlResponse() );
			} );

			// Act
			await this._throttler.ExecuteWithPaginationAsync( funcToThrottle, Mark.Create );

			// Assert
			Assert.That( funcToThrottleCallCount, Is.EqualTo( 1 ) );
		}

		[ Test ]
		public void ExecuteWithPaginationAsync_RetryMaxRetryCountAndThrowsThrottlerException_WhenFunctionFailedWithThrottledCode()
		{
			// Arrange
			var funcToThrottleCallCount = 0;
			var funcToThrottle = new Func< Task< GraphQlResponseWithPages< GetProductsData, Product > > >( () =>
			{
				funcToThrottleCallCount++;
				return Task.FromResult( GetPaginatedThrottledErrorResponse() );
			} );

			// Act, Assert
			Assert.Multiple( () => {
				Assert.ThrowsAsync< ThrottlerException >( async () => {
					await this._throttler.ExecuteWithPaginationAsync( funcToThrottle, Mark.Create );
				} );
				Assert.That( funcToThrottleCallCount, Is.EqualTo( MaxRetryCount ) );
			});
		}

		[ Test ]
		public void ExecuteWithPaginationAsync_ThrowsSystemException_WhenFunctionFailedWithUnknownError()
		{
			// Arrange
			var funcToThrottleCallCount = 0;
			var funcToThrottle = new Func< Task< GraphQlResponseWithPages< GetProductsData, Product > > >( () =>
			{
				funcToThrottleCallCount++;
				return Task.FromResult( GetPaginatedUnknownErrorResponse() );
			} );

			// Act, Assert
			Assert.Multiple( () => {
				Assert.ThrowsAsync< SystemException >( async () => {
					await this._throttler.ExecuteWithPaginationAsync( funcToThrottle, Mark.Create );
				} );
				Assert.That( funcToThrottleCallCount, Is.EqualTo( 1 ) );
			});
		}
		
		[ Test ]
 		public async Task ExecuteWithPaginationAsync_RetryAndReturnsResult_WhenFunctionFailedFirstTimeWithThrottledCode_AndSuccessSecondTime()
		{
			// Arrange
			var funcToThrottleCallCount = 0;
			var funcToThrottle = new Func< Task< GraphQlResponseWithPages< GetProductsData, Product > > >( () =>
			{
				funcToThrottleCallCount++;
				return Task.FromResult( funcToThrottleCallCount == 0 ? GetPaginatedThrottledErrorResponse() : GetPaginatedGraphQlResponse() );
			} );

			// Act
			await this._throttler.ExecuteWithPaginationAsync( funcToThrottle, Mark.Create );

			// Assert
			Assert.That( funcToThrottleCallCount, Is.EqualTo( 1 ) );
		}

		[ Test ]
		public async Task WaitIfNeededAsync_DontWait_WhenCurrentlyAvailableMoreThanRequestedQueryCost()
		{
			// Arrange
			var response = this.GetBaseGraphQlResponse();

			// Act
			var result = await ShopifyGraphQlThrottler.WaitIfNeededAsync( response );

			// Assert
			result.Should().Be( 0 );
		}

		[ Test ]
		public async Task WaitIfNeededAsync_WaitNeededTime_WhenCurrentlyAvailableLessThanRequestedQueryCost()
		{
			// Arrange
			var response = this.GetBaseGraphQlResponse();
			response.Extensions.Cost.RequestedQueryCost = 9;
			response.Extensions.Cost.ThrottleStatus.CurrentlyAvailable = 5;
			response.Extensions.Cost.ThrottleStatus.RestoreRate = 3;

			// Act
			var result = await ShopifyGraphQlThrottler.WaitIfNeededAsync( response );

			// Assert
			result.Should().Be( 2000 ); // 2 seconds
		}

		private BaseGraphQlResponse< ProductVariant > GetBaseGraphQlResponse()
		{
			return new BaseGraphQlResponse< ProductVariant >
			{
				Errors = null,
				Extensions = new GraphQlExtensions
				{
					Cost = new Cost
					{
						ActualQueryCost = 1,
						RequestedQueryCost = 1,
						ThrottleStatus = new ThrottleStatus
						{
							CurrentlyAvailable = 1000,
							MaximumAvailable = 1000,
							RestoreRate = 50
						}
					}
				}
			};
		}
		
		private static GraphQlResponseWithPages< GetProductsData, Product > GetPaginatedGraphQlResponse()
		{
			return new GetProductsResponse
			{
				Errors = null,
				Extensions = new GraphQlExtensions
				{
					Cost = new Cost
					{
						ActualQueryCost = 1,
						RequestedQueryCost = 1,
						ThrottleStatus = new ThrottleStatus
						{
							CurrentlyAvailable = 1000,
							MaximumAvailable = 1000,
							RestoreRate = 50
						}
					}
				}
			};
		}

		private BaseGraphQlResponse< ProductVariant > GetThrottledErrorResponse()
		{
			var response = this.GetBaseGraphQlResponse();
			response.Errors = new[]
			{
				new GraphQlError()
				{
					Message = "Throttled",
					Extensions = new GraphQlErrorExtensions()
					{
						Code = "THROTTLED"
					}
				}
			};

			return response;
		}
		
		private static GraphQlResponseWithPages< GetProductsData, Product > GetPaginatedThrottledErrorResponse()
		{
			var response = GetPaginatedGraphQlResponse();
			response.Errors = new[]
			{
				new GraphQlError
				{
					Message = "Throttled",
					Extensions = new GraphQlErrorExtensions
					{
						Code = "THROTTLED"
					}
				}
			};

			return response;
		}

		private BaseGraphQlResponse< ProductVariant > GetUnknownErrorResponse()
		{
			var response = this.GetBaseGraphQlResponse();
			response.Errors = new[]
			{
				new GraphQlError()
				{
					Message = "Unknown",
					Extensions = new GraphQlErrorExtensions()
					{
						Code = "Unknown"
					}
				}
			};

			return response;
		}
		
		private static GraphQlResponseWithPages< GetProductsData, Product > GetPaginatedUnknownErrorResponse()
		{
			var response = GetPaginatedGraphQlResponse();
			response.Errors = new[]
			{
				new GraphQlError()
				{
					Message = "Unknown",
					Extensions = new GraphQlErrorExtensions()
					{
						Code = "Unknown"
					}
				}
			};

			return response;
		}
	}
}