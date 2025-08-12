namespace ShopifyAccess.GraphQl.Queries
{
	public static class GetOrdersQuery
	{
		public static readonly string Query = @"query GetOrdersPaginated($query: String!, $first: Int!, $after: String) {
  orders(first: $first, after: $after, query: $query) {
    nodes {
      id
      name
      number
      createdAt
      totalPriceSet {
        shopMoney {
          amount
          currencyCode
        }
      }
      lineItems(first: 250) {
        nodes {
          id
          title
          quantity
          sku
          originalUnitPriceSet {
            shopMoney {
              amount
              currencyCode
            }
          }
          totalDiscountSet {
            shopMoney {
              amount
              currencyCode
            }
          }
          taxLines {
            title
            priceSet {
              shopMoney {
                amount
                currencyCode
              }
            }
          }
        }
      }
      closedAt
      cancelledAt
      displayFinancialStatus
      displayFulfillmentStatus
      fulfillments(first: 250) {
        createdAt
        status
        id
        order {
          id
        }
        updatedAt
        location {
          id
        }
        trackingInfo {
          company
          number
          url
        }
        fulfillmentLineItems(first: 250) {
          nodes {
            lineItem {
              id
              sku
              quantity
              originalUnitPriceSet {
                shopMoney {
                  amount
                  currencyCode
                }
              }
            }
          }
        }
      }
      sourceName
      shippingLine {
        title
        originalPriceSet {
          shopMoney {
            amount
            currencyCode
          }
        }
        id
        code
        source
      }
      discountCodes
      taxLines {
        title
        priceSet {
          shopMoney {
            amount
          }
        }
      }
      refunds {
        id
        refundLineItems(first: 250) {
          nodes {
            id
            lineItem {
              id
            }
            quantity
            restockType
          }
        }
      }
    }
    pageInfo {
      hasNextPage
      endCursor
    }
  }
}";
	}
}
