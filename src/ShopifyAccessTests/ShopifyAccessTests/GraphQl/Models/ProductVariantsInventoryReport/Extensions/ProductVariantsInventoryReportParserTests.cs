using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.GraphQl.Models.ProductVariantsInventoryReport.Extensions;

namespace ShopifyAccessTests.GraphQl.Models.ProductVariantsInventoryReport.Extensions
{
	[ TestFixture ]
	public class ProductVariantsInventoryReportParserTests
	{
		private const string Response =
			@"{""id"":""gid://shopify/ProductVariant/337095344"",""sku"":""testsku1"",""inventoryItem"":{""id"":""gid://shopify/InventoryItem/2860696897"",""tracked"":true}}
			{""available"":10,""location"":{""id"":""gid://shopify/Location/43176333"",""name"":""10107 Production Court""},""__parentId"":""gid://shopify/ProductVariant/337095344""}
			{""available"":20,""location"":{""id"":""gid://shopify/Location/1836161"",""name"":""10107 Production Ct.""},""__parentId"":""gid://shopify/ProductVariant/337095344""}
			{""id"":""gid://shopify/ProductVariant/337095346"",""sku"":""testsku2"",""inventoryItem"":{""id"":""gid://shopify/InventoryItem/2860696961"",""tracked"":false}}
			{""available"":30,""location"":{""id"":""gid://shopify/Location/17468227646"",""name"":""Is This a POS Location""},""__parentId"":""gid://shopify/ProductVariant/337095346""}
			{""available"":40,""location"":{""id"":""gid://shopify/Location/36696031294"",""name"":""TestLocation""},""__parentId"":""gid://shopify/ProductVariant/337095346""}";

		[ Test ]
		public void Parse_ReturnsListOfProductVariants_WhenStreamProvided()
		{
			// Arrange
			var stream = this.GetStreamFromString( Response );

			// Act
			var productVariants = ProductVariantsInventoryReportParser.Parse( stream ).ToList();

			// Assert
			productVariants.Should().HaveCount( 2 );
			productVariants[ 0 ].ProductVariantId.Should().Be( "gid://shopify/ProductVariant/337095344" );
			productVariants[ 0 ].Sku.Should().Be( "testsku1" );
			productVariants[ 0 ].InventoryItem.Tracked.Should().BeTrue();
			productVariants[ 0 ].InventoryItem.InventoryItemId.Should().Be( "gid://shopify/InventoryItem/2860696897" );
			productVariants[ 0 ].InventoryLevels.Should().HaveCount( 2 );
			productVariants[ 0 ].InventoryLevels[ 0 ].Available.Should().Be( 10 );
			productVariants[ 0 ].InventoryLevels[ 0 ].Location.LocationId.Should().Be( "gid://shopify/Location/43176333" );
			productVariants[ 0 ].InventoryLevels[ 1 ].Available.Should().Be( 20 );
			productVariants[ 0 ].InventoryLevels[ 1 ].Location.LocationId.Should().Be( "gid://shopify/Location/1836161" );

			productVariants[ 1 ].ProductVariantId.Should().Be( "gid://shopify/ProductVariant/337095346" );
			productVariants[ 1 ].Sku.Should().Be( "testsku2" );
			productVariants[ 1 ].InventoryItem.Tracked.Should().BeFalse();
			productVariants[ 1 ].InventoryItem.InventoryItemId.Should().Be( "gid://shopify/InventoryItem/2860696961" );
			productVariants[ 1 ].InventoryLevels.Should().HaveCount( 2 );
			productVariants[ 1 ].InventoryLevels[ 0 ].Available.Should().Be( 30 );
			productVariants[ 1 ].InventoryLevels[ 0 ].Location.LocationId.Should().Be( "gid://shopify/Location/17468227646" );
			productVariants[ 1 ].InventoryLevels[ 1 ].Available.Should().Be( 40 );
			productVariants[ 1 ].InventoryLevels[ 1 ].Location.LocationId.Should().Be( "gid://shopify/Location/36696031294" );
		}

		private Stream GetStreamFromString( string s )
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter( stream );
			writer.Write( s );
			writer.Flush();
			stream.Position = 0;
			return stream;
		}
	}
}