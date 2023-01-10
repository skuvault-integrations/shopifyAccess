using System.Configuration;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.GraphQl;
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
				this.TestReportGenerator = new TestReportGenerator( webRequestServices );
			}
			else
			{
				throw new ConfigurationErrorsException();
			}
		}

		[ Test ]
		public async Task GenerateRequestAsync_ReturnsBulkOperationStatus()
		{
			// Arrange
			// Act
			var currentBulkOperation = await this.TestReportGenerator.GenerateRequestAsync( ReportType.ProductVariantsWithInventoryLevels );

			// Assert
			currentBulkOperation.Status.Should().Be( "CREATED" );
		}
		
		[ Test ]
		public async Task GetCurrentBulkOperationAsync_ReturnsLastOperationStatus()
		{
			// Arrange
			// Act
			var currentBulkOperation = await this.TestReportGenerator.GetCurrentBulkOperationAsync();

			// Assert
			currentBulkOperation.Status.Should().Be( "COMPLETED" );
			currentBulkOperation.ErrorCode.Should().BeNull();
		}
	}
}