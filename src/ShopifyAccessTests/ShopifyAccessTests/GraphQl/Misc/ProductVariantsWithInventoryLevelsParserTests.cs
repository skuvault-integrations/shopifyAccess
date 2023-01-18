using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using ShopifyAccess.GraphQl.Misc;

namespace ShopifyAccessTests.GraphQl.Misc
{
	[ TestFixture ]
	public class ProductVariantsWithInventoryLevelsParserTests
	{
		private const string Response =
			@"{""id"":""gid://shopify/ProductVariant/337095344"",""sku"":""testsku1"",""inventoryItem"":{""id"":""gid://shopify/InventoryItem/2860696897"",""tracked"":true}}
			{""available"":10,""location"":{""id"":""gid://shopify/Location/43176333"",""name"":""10107 Production Court""},""__parentId"":""gid://shopify/ProductVariant/337095344""}
			{""available"":20,""location"":{""id"":""gid://shopify/Location/1836161"",""name"":""10107 Production Ct.""},""__parentId"":""gid://shopify/ProductVariant/337095344""}
			{""id"":""gid://shopify/ProductVariant/337095346"",""sku"":""testsku2"",""inventoryItem"":{""id"":""gid://shopify/InventoryItem/2860696961"",""tracked"":false}}
			{""available"":30,""location"":{""id"":""gid://shopify/Location/17468227646"",""name"":""Is This a POS Location""},""__parentId"":""gid://shopify/ProductVariant/337095346""}
			{""available"":40,""location"":{""id"":""gid://shopify/Location/36696031294"",""name"":""TestLocation""},""__parentId"":""gid://shopify/ProductVariant/337095346""}";

		[ Test ]
		public void Parse_ReturnsReport_WhenStreamProvided()
		{
			// Arrange
			var stream = this.GetStreamFromString( Response );

			// Act
			var report = ProductVariantsWithInventoryLevelsParser.Parse( stream ).ToList();

			// Assert
			report.Should().HaveCount( 2 );
			report[ 0 ].ProductVariantId.Should().Be( "gid://shopify/ProductVariant/337095344" );
			report[ 0 ].Sku.Should().Be( "testsku1" );
			report[ 0 ].InventoryItem.Tracked.Should().BeTrue();
			report[ 0 ].InventoryItem.InventoryItemId.Should().Be( "gid://shopify/InventoryItem/2860696897" );
			report[ 0 ].InventoryLevels.Should().HaveCount( 2 );
			report[ 0 ].InventoryLevels[ 0 ].Available.Should().Be( 10 );
			report[ 0 ].InventoryLevels[ 0 ].Location.LocationId.Should().Be( "gid://shopify/Location/43176333" );
			report[ 0 ].InventoryLevels[ 1 ].Available.Should().Be( 20 );
			report[ 0 ].InventoryLevels[ 1 ].Location.LocationId.Should().Be( "gid://shopify/Location/1836161" );

			report[ 1 ].ProductVariantId.Should().Be( "gid://shopify/ProductVariant/337095346" );
			report[ 1 ].Sku.Should().Be( "testsku2" );
			report[ 1 ].InventoryItem.Tracked.Should().BeFalse();
			report[ 1 ].InventoryItem.InventoryItemId.Should().Be( "gid://shopify/InventoryItem/2860696961" );
			report[ 1 ].InventoryLevels.Should().HaveCount( 2 );
			report[ 1 ].InventoryLevels[ 0 ].Available.Should().Be( 30 );
			report[ 1 ].InventoryLevels[ 0 ].Location.LocationId.Should().Be( "gid://shopify/Location/17468227646" );
			report[ 1 ].InventoryLevels[ 1 ].Available.Should().Be( 40 );
			report[ 1 ].InventoryLevels[ 1 ].Location.LocationId.Should().Be( "gid://shopify/Location/36696031294" );
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