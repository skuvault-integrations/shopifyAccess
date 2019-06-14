namespace ShopifyAccess.Models.Configuration.Command
{
    public class ShopifyProductCommandEndpointName
    {
	    public string Name { get; private set; }

	    public static readonly ShopifyProductCommandEndpointName ProductDateUpdatedAfter = new ShopifyProductCommandEndpointName( "updated_at_min" );
	    public static readonly ShopifyProductCommandEndpointName ProductDateCreatedAfter = new ShopifyProductCommandEndpointName( "created_at_min" );
	    public static readonly ShopifyProductCommandEndpointName ProductDateCreatedBefore = new ShopifyProductCommandEndpointName( "created_at_max" );

	    private ShopifyProductCommandEndpointName( string name )
	    {
		    this.Name = name;
	    }
    }
}
