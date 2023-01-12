using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.GraphQl;
using ShopifyAccess.GraphQl.Models.ProductVariantsWithInventoryLevelsReport;
using ShopifyAccess.Models;
using ShopifyAccess.Services;

namespace ShopifyAccessTests.GraphQl.Services
{
	[ TestFixture ]
	public class ReportGeneratorTests: BaseTests
	{
		private TestReportGenerator TestReportGenerator;

		[ SetUp ]
		public void InitReportGeneratorTests()
		{
			if( this.Config != null )
			{
				var webRequestServices = new WebRequestServices( this.Config );
				this.TestReportGenerator = new TestReportGenerator( this.Config .ShopName, webRequestServices );
			}
			else
			{
				throw new ConfigurationErrorsException();
			}
		}

		[ Test ]
		[ Explicit ]
		public async Task GenerateRequestAsync_ReturnsBulkOperationStatus()
		{
			// Arrange
			// Act
			var currentBulkOperation = await this.TestReportGenerator.GenerateRequestAsync( ReportType.ProductVariantsWithInventoryLevels );

			// Assert
			currentBulkOperation.Status.Should().Be( "CREATED" );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetCurrentBulkOperationAsync_ReturnsLastOperationStatus()
		{
			// Arrange
			// Act
			var currentBulkOperation = await this.TestReportGenerator.GetCurrentBulkOperationAsync();

			// Assert
			currentBulkOperation.Status.Should().Be( "COMPLETED" );
			currentBulkOperation.ErrorCode.Should().BeNull();
		}

		[ Test ]
		[ Explicit ]
		public async Task GetReportDocumentAsync_ReturnsReportLines()
		{
			// Arrange
			var currentBulkOperation = await this.TestReportGenerator.GetCurrentBulkOperationAsync();
			var url = currentBulkOperation.Url;

			// Act
			var reportLines = await this.TestReportGenerator.GetReportDocumentAsync< ProductVariant >( ReportType.ProductVariantsWithInventoryLevels, url );

			// Assert
			reportLines.Should().NotBeEmpty();
		}
		
		[ Test ]
		[ Explicit ]
		public async Task GetReportAsync_ReturnsReportLines()
		{
			// Arrange
			// Act
			var reportLines = await this.TestReportGenerator.GetReportAsync< ProductVariant >( ReportType.ProductVariantsWithInventoryLevels, 10000, CancellationToken.None, Mark.Create );

			// Assert
			reportLines.Should().NotBeEmpty();
		}
	}
}