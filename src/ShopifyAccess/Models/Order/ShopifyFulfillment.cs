namespace ShopifyAccess.Models.Order
{
	public class ShopifyFulfillment
	{
		public long Id { get; set; }
		public long OrderId { get; set; }
		public ShopifyOrderFulfillmentStatus Status { get; set; }
	}

	// ReSharper disable InconsistentNaming
	public enum ShopifyOrderFulfillmentStatus
	{
		Undefined,
		shipped,
		partial,
		unshipped,
		any
	}

	public enum ShopifyOrderStatus
	{
		Undefined,
		open,
		closed,
		cancelled,
		any
	}

	public enum ShopifyOrderFinancialStatus
	{
		Undefined,
		authorized,
		pending,
		paid,
		partially_paid,
		abandoned,
		refunded,
		voided,
		any
	}

	// ReSharper restore InconsistentNaming
}