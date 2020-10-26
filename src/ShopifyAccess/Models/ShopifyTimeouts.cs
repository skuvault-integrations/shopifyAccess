using System.Collections.Generic;
using CuttingEdge.Conditions;

namespace ShopifyAccess.Models
{
	public enum ShopifyOperationEnum
	{
		GetProductsCount,
		GetProducts,
		GetProductsInventory,
		UpdateInventory,
		UpdateProductVariantQuantity,
		GetOrdersCount,
		GetOrders,
		GetLocations,
		GetUsers,
		GetUser
	}

	public class ShopifyOperationTimeout
	{
		public int TimeoutInMs { get; private set; }

		public ShopifyOperationTimeout( int timeoutInMs )
		{
			Condition.Requires( timeoutInMs, "timeoutInMs" ).IsGreaterThan( 0 );
			this.TimeoutInMs = timeoutInMs;
		}
	}

	public class ShopifyTimeouts
	{
		private const int DefaultTimeoutInMs = 10 * 60 * 1000;
		private readonly Dictionary< ShopifyOperationEnum, ShopifyOperationTimeout > _timeouts;

		/// <summary>
		///	This timeout value will be used if specific timeout for operation is not provided. Default value can be changed through constructor.
		/// </summary>
		private ShopifyOperationTimeout DefaultOperationTimeout { get; set; }

		public int this[ ShopifyOperationEnum operation ]
		{
			get
			{
				ShopifyOperationTimeout timeout;
				if ( this._timeouts.TryGetValue( operation, out timeout ) )
					return timeout.TimeoutInMs;

				return this.DefaultOperationTimeout.TimeoutInMs;
			}
		}

		public void Set( ShopifyOperationEnum operation, ShopifyOperationTimeout timeout )
		{
			this._timeouts[ operation ] = timeout;
		}

		public ShopifyTimeouts( int defaultTimeoutInMs )
		{
			this._timeouts = new Dictionary< ShopifyOperationEnum, ShopifyOperationTimeout >();
			this.DefaultOperationTimeout = new ShopifyOperationTimeout( defaultTimeoutInMs );
		}

		public ShopifyTimeouts() : this( DefaultTimeoutInMs ) { }
	}
}