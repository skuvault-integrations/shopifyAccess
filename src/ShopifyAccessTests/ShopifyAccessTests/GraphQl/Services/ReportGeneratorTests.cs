using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.GraphQl;
using ShopifyAccess.GraphQl.Models.ProductVariantsInventoryReport.Extensions;
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
				this.TestReportGenerator = new TestReportGenerator( this.Config.ShopName, webRequestServices );
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
			var currentBulkOperation = await this.TestReportGenerator.GenerateRequestAsync( ReportType.ProductVariantsInventory );

			// Assert
			currentBulkOperation.Status.Should().Be( BulkOperationStatus.Created.ToString().ToUpperInvariant() );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetCurrentBulkOperationAsync_ReturnsLastOperationStatus()
		{
			// Arrange
			// Act
			var currentBulkOperation = await this.TestReportGenerator.GetCurrentBulkOperationAsync();

			// Assert
			currentBulkOperation.Status.Should().Be( BulkOperationStatus.Completed.ToString().ToUpperInvariant() );
			currentBulkOperation.ErrorCode.Should().BeNull();
		}

		[ Test ]
		[ Explicit ]
		public async Task GetBulkOperationStatusByIdAsync_ReturnsCurrentBulkOperationStatus_WhenCurrentBulkOperationGidProvided()
		{
			// Arrange
			var currentBulkOperation = await this.TestReportGenerator.GetCurrentBulkOperationAsync();

			// Act
			var bulkOperationById = await this.TestReportGenerator.GetBulkOperationByIdAsync( currentBulkOperation.Id );

			// Assert
			bulkOperationById.Should().BeEquivalentTo( currentBulkOperation );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetReportDocumentAsync_ReturnsReportLines()
		{
			// Arrange
			var currentBulkOperation = await this.TestReportGenerator.GetCurrentBulkOperationAsync();
			var url = currentBulkOperation.Url;

			// Act
			var reportLines = await this.TestReportGenerator.GetReportDocumentAsync( ProductVariantsInventoryReportParser.Parse, url );

			// Assert
			reportLines.Should().NotBeEmpty();
		}

		[ Test ]
		[ Explicit ]
		public async Task GetReportAsync_ReturnsReportLines()
		{
			// Arrange
			// Act
			var reportLines = await this.TestReportGenerator.GetReportAsync( ReportType.ProductVariantsInventory,
				ProductVariantsInventoryReportParser.Parse,
				10000,
				Mark.Create, CancellationToken.None );

			// Assert
			reportLines.Should().NotBeEmpty();
		}
	}
}