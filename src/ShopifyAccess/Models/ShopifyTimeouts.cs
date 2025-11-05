using System;
using System.Collections.Generic;

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
			if( timeoutInMs <= 0 )
			{
				throw new ArgumentOutOfRangeException( nameof(timeoutInMs), timeoutInMs, "timeoutInMs must be greater than 0" );
			}

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
		public ShopifyOperationTimeout DefaultOperationTimeout { get; private set; }

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