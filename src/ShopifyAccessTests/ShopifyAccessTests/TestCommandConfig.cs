using LINQtoCSV;

namespace ShopifyAccessTests
{
	internal class TestCommandConfig
	{
		[ CsvColumn( Name = "ShopName", FieldIndex = 1 ) ]
		public string ShopName { get; set; }

		[ CsvColumn( Name = "AccessToken", FieldIndex = 2 ) ]
		public string AccessToken { get; set; }
	}
}