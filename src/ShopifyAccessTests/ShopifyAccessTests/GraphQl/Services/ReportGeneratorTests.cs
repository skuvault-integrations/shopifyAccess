using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.GraphQl.Services;
using ShopifyAccess.Models;
using ShopifyAccess.Services;

namespace ShopifyAccessTests.GraphQl.Services
{
	[ TestFixture ]
	public class ReportGeneratorTests: BaseTests
	{
		private ReportGenerator ReportGenerator;

		[ SetUp ]
		public void InitReportGeneratorTests()
		{
			if( this.Config != null )
			{
				var webRequestServices = new WebRequestServices( this.Config );
				this.ReportGenerator = new ReportGenerator( webRequestServices );
			}
			else
			{
				throw new ConfigurationException();
			}
		}

		[ Test ]
		public async Task GetCurrentBulkOperationAsync_ReturnsLastOperationStatus()
		{
			// Arrange
			var mark = Mark.Create;

			// Act
			var currentBulkOperation = await this.ReportGenerator.GetCurrentBulkOperationAsync( CancellationToken.None, mark );

			// Assert
			currentBulkOperation.Status.Should().Be( "COMPLETED" );
			currentBulkOperation.ErrorCode.Should().BeNull();
		}
	}
}