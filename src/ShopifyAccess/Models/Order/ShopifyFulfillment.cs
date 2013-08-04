namespace ShopifyAccess.Models.Order
{
	public class ShopifyFulfillment
	{
		public long Id { get; set; }
		public long OrderId { get; set; }
		public ShopifyOrderFulfillmentStatus Status { get; set; }
	}

	public enum ShopifyOrderFulfillmentStatus
	{
		// ReSharper disable InconsistentNaming
		Undefined,
		shipped,
		partial,
		unshipped,
		any
		// ReSharper restore InconsistentNaming
	}
}