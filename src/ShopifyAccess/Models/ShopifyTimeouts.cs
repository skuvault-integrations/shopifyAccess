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
		UpdateProductVariant,
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
		public const int DefaultTimeoutInMs = 10 * 60 * 1000;
		private Dictionary< ShopifyOperationEnum, ShopifyOperationTimeout > _timeouts;

		/// <summary>
		///	This timeout value will be used if specific timeout for operation is not provided. Default value can be changed through constructor.
		/// </summary>
		public ShopifyOperationTimeout DefaultOperationTimeout { get; private set; }

		public int this[ ShopifyOperationEnum operation ]
		{
			get
			{
				if ( this._timeouts.TryGetValue( operation, out ShopifyOperationTimeout timeout ) )
					return timeout.TimeoutInMs;

				return this.DefaultOperationTimeout.TimeoutInMs;
			}
		}

		public void Set( ShopifyOperationEnum operation, ShopifyOperationTimeout timeout )
		{
			if ( this._timeouts.ContainsKey( operation ) )
			{
				this._timeouts.Remove( operation );
			}

			this._timeouts.Add( operation, timeout );
		}

		public ShopifyTimeouts( int defaultTimeoutInMs )
		{
			this._timeouts = new Dictionary< ShopifyOperationEnum, ShopifyOperationTimeout >();
			this.DefaultOperationTimeout = new ShopifyOperationTimeout( defaultTimeoutInMs );
		}

		public ShopifyTimeouts() : this( DefaultTimeoutInMs ) { }
	}
}