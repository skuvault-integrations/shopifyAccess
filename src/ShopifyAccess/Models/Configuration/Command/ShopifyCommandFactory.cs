namespace ShopifyAccess.Models.Configuration.Command
{
	public class ShopifyCommandFactory
	{
		private readonly ShopifyApiVersion _apiVersion;

		public ShopifyCommand CreateUnknownCommand() => new ShopifyCommand( string.Empty );
		public ShopifyCommand CreateUpdateInventoryLevelsCommand() => new ShopifyCommand( "/inventory_levels/set.json", this._apiVersion );
		public ShopifyCommand CreateGetProductsCommand() => new ShopifyCommand( "/products.json", this._apiVersion );
		public ShopifyCommand CreateGetProductsCountCommand() => new ShopifyCommand( "/products/count.json", this._apiVersion );
		public ShopifyCommand CreateGetInventoryLevelsCommand() => new ShopifyCommand( "/inventory_levels.json", this._apiVersion );
		public ShopifyCommand CreateGetOrdersCountCommand() => new ShopifyCommand( "/orders/count.json", this._apiVersion );
		public ShopifyCommand CreateGetOrdersCommand() => new ShopifyCommand( "/orders.json", this._apiVersion );
		public ShopifyCommand CreateGetLocationsCommand() => new ShopifyCommand( "/locations.json", this._apiVersion );
		public ShopifyCommand CreateGetUsersCommand() => new ShopifyCommand( "/users.json", this._apiVersion );
		public ShopifyCommand CreateGraphQlCommand() => new ShopifyCommand( "/graphql.json", this._apiVersion );

		public ShopifyCommandFactory( ShopifyApiVersion apiVersion )
		{
			this._apiVersion = apiVersion;
		}
	}
}