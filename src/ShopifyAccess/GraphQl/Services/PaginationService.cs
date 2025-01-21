using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShopifyAccess.GraphQl.Models;
using ShopifyAccess.Misc;
using ShopifyAccess.Models;

namespace ShopifyAccess.GraphQl.Services
{
	internal class PaginationService
	{
		private readonly ShopifyGraphQlThrottler _graphQlThrottler;
		private readonly string _shopName;

		public PaginationService( ShopifyGraphQlThrottler graphQlThrottler, string shopName )
		{
			this._graphQlThrottler = graphQlThrottler;
			this._shopName = shopName;
		}

		/// <summary>
		/// Get all pages of multi-page responses
		/// </summary>
		/// <param name="sendRequestAsync"></param>
		/// <param name="mark"></param>
		/// <param name="token"></param>
		/// <typeparam name="TData">GraphQL response "data" element type</typeparam>
		/// <typeparam name="TResponseItem"></typeparam>
		/// <returns></returns>
		internal async Task< List< TResponseItem > > GetAllPagesAsync< TData, TResponseItem >( Func< string, Task< GraphQlResponseWithPages< TData, TResponseItem > > > sendRequestAsync,
			Mark mark, CancellationToken token )
		{
			string nextCursor = null;

			var result = new List< TResponseItem >();
			do
			{
				var response = await ActionPolicies.GetPolicyAsync( mark, this._shopName, token ).Get(
					() => this._graphQlThrottler.ExecuteWithPaginationAsync(
						async () => await sendRequestAsync( nextCursor ),
						mark )
				).ConfigureAwait( false );

				var itemsAndPagingInfo = response.GetItemsAndPagingInfo();
				
				result.AddRange( itemsAndPagingInfo.Items );

				if( itemsAndPagingInfo.PageInfo.HasNextPage )
				{
					nextCursor = itemsAndPagingInfo.PageInfo.EndCursor;
				}
				else
				{
					break;
				}
			} while( true );

			return result;
		}
	}
}