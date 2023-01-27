using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.GraphQl;
using ShopifyAccess.GraphQl.Models.ProductVariantsInventory.Extensions;
using ShopifyAccess.GraphQl.Services;
using ShopifyAccess.Models;
using ShopifyAccess.Models.Configuration.Command;
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
				var shopifyCommandFactory = new ShopifyCommandFactory( ShopifyApiVersion.V2022_07 );
				this.ReportGenerator = new ReportGenerator( this.Config.ShopName, webRequestServices, shopifyCommandFactory );
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
			var currentBulkOperation = await this.ReportGenerator.GenerateReportAsync( ReportType.ProductVariantsInventory, Mark.Create, CancellationToken.None );

			// Assert
			currentBulkOperation.Status.Should().Be( BulkOperationStatus.Created.ToString().ToUpperInvariant() );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetCurrentBulkOperationAsync_ReturnsLastOperationStatus()
		{
			// Arrange
			// Act
			var currentBulkOperation = await this.ReportGenerator.GetCurrentBulkOperationAsync( Mark.Create, CancellationToken.None );

			// Assert
			currentBulkOperation.Status.Should().Be( BulkOperationStatus.Completed.ToString().ToUpperInvariant() );
			currentBulkOperation.ErrorCode.Should().BeNull();
		}

		[ Test ]
		[ Explicit ]
		public async Task GetBulkOperationStatusByIdAsync_ReturnsCurrentBulkOperationStatus_WhenCurrentBulkOperationGidProvided()
		{
			// Arrange
			var currentBulkOperation = await this.ReportGenerator.GetCurrentBulkOperationAsync( Mark.Create, CancellationToken.None );

			// Act
			var bulkOperationById = await this.ReportGenerator.GetBulkOperationByIdAsync( currentBulkOperation.Id, Mark.Create, CancellationToken.None );

			// Assert
			bulkOperationById.Should().BeEquivalentTo( currentBulkOperation );
		}

		[ Test ]
		[ Explicit ]
		public async Task GetReportDocumentAsync_ReturnsReportLines()
		{
			// Arrange
			var currentBulkOperation = await this.ReportGenerator.GetCurrentBulkOperationAsync( Mark.Create, CancellationToken.None );
			var url = currentBulkOperation.Url;
			var timeout = 100000;

			// Act
			var reportLines = await this.ReportGenerator.GetReportDocumentAsync( url, ProductVariantsInventoryReportParser.Parse, timeout, Mark.Create, CancellationToken.None );

			// Assert
			reportLines.Should().NotBeEmpty();
		}

		[ Test ]
		[ Explicit ]
		public async Task GetReportAsync_ReturnsReportLines()
		{
			// Arrange
			var timeout = 100000;

			// Act
			var reportLines = await this.ReportGenerator.GetReportAsync( ReportType.ProductVariantsInventory,
				ProductVariantsInventoryReportParser.Parse,
				timeout,
				Mark.Create, CancellationToken.None );

			// Assert
			reportLines.Should().NotBeEmpty();
		}
	}
}