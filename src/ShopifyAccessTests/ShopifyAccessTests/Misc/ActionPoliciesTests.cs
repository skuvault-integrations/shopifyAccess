using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Netco.ThrottlerServices;
using NUnit.Framework;
using ShopifyAccess.Exceptions;
using ShopifyAccess.Misc;
using ShopifyAccess.Models;

namespace ShopifyAccessTests.Misc
{
	[ TestFixture ]
	public class ActionPoliciesTests
	{
		private const string ShopName = "TestShop";

		[ Test ]
		public void GetPolicy_RetryAction_WhenShopifyTransientExceptionOneTime()
		{
			// Arrange
			var counter = 0;
			Func< bool > action = () =>
			{
				// First time throw ShopifyTransientException
				if( ++counter == 1 )
					throw new ShopifyTransientException( "", 500 );
				return false;
			};

			// Act
			ActionPolicies.GetPolicy( Mark.Create, ShopName, CancellationToken.None ).Get( action );

			// Assert
			counter.Should().Be( 2 );
		}

		[ Test ]
		public void GetPolicy_RetryAction_WhenTaskCanceledException()
		{
			// Arrange
			var counter = 0;
			var cts = new CancellationTokenSource();
			Func< bool > action = () =>
			{
				// First time throw TaskCanceledException
				if( ++counter == 1 )
					throw new TaskCanceledException();
				return false;
			};

			// Act
			ActionPolicies.GetPolicy( Mark.Create, ShopName, cts.Token ).Get( action );

			// Assert
			counter.Should().Be( 2 );
		}

		[ Test ]
		public void GetPolicy_RetryActionAndThenThrowsShopifyTransientException_WhenShopifyTransientException()
		{
			// Arrange
			var counter = 0;
			Func< bool > action = () =>
			{
				++counter;
				throw new ShopifyTransientException( "", 500 );
			};

			// Act, Assert
			ActionPolicies.GetPolicy( Mark.Create, ShopName, CancellationToken.None ).Invoking( p => p.Get( action ) ).Should().Throw< ShopifyTransientException >();
			counter.Should().Be( 2 );
		}

		[ Test ]
		public void GetPolicy_DoesNotRetryAction_WhenUnauthorizedException()
		{
			// Arrange
			var counter = 0;
			Func< bool > action = () =>
			{
				// First time throw ShopifyUnauthorizedException
				if( ++counter == 1 )
					throw new ShopifyUnauthorizedException( "", 401 );
				return false;
			};

			// Act, Assert
			ActionPolicies.GetPolicy( Mark.Create, ShopName, CancellationToken.None ).Invoking( p => p.Get( action ) ).Should().Throw< ShopifyUnauthorizedException >();
			counter.Should().Be( 1 );
		}

		[ Test ]
		public void GetPolicy_DoesNotRetryAction_WhenThrottlerException()
		{
			// Arrange
			var counter = 0;
			Func< bool > action = () =>
			{
				// First time throw ShopifyUnauthorizedException
				if( ++counter == 1 )
					throw new ThrottlerException( "" );
				return false;
			};

			// Act, Assert
			ActionPolicies.GetPolicy( Mark.Create, ShopName, CancellationToken.None ).Invoking( p => p.Get( action ) ).Should().Throw< ThrottlerException >();
			counter.Should().Be( 1 );
		}

		[ Test ]
		public void GetPolicy_DoesNotRetryAction_WhenException()
		{
			// Arrange
			var counter = 0;
			Func< bool > action = () =>
			{
				// First time throw ShopifyUnauthorizedException
				if( ++counter == 1 )
					throw new Exception();
				return false;
			};

			// Act, Assert
			ActionPolicies.GetPolicy( Mark.Create, ShopName, CancellationToken.None ).Invoking( p => p.Get( action ) ).Should().Throw< Exception >();
			counter.Should().Be( 1 );
		}
	}
}