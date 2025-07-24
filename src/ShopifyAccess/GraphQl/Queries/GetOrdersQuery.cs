namespace ShopifyAccess.GraphQl.Queries
{
	public static class GetOrdersQuery
	{
		public static readonly string Query = @"
query getOrders($first: Int!, $after: String, $createdAtMin: DateTime!, $createdAtMax: DateTime, $status: String) {
  orders(first: $first, after: $after, query: ""created_at:>=$createdAtMin created_at:<=$createdAtMax financial_status:$status"") {
    edges {
      cursor
      node {
        id
        name
        orderNumber
        createdAt
        closedAt
        cancelledAt
        financialStatus
        totalPriceSet {
          shopMoney {
            amount
            currencyCode
          }
        }
        billingAddress {
          address1
          address2
          city
          country
          zip
        }
        shippingAddress {
          address1
          address2
          city
          country
          zip
        }
        fulfillments {
          status
          trackingCompany
          trackingInfo {
            number
            url
          }
        }
        lineItems(first: 100) {
          edges {
            node {
              id
              title
              sku
              quantity
              originalUnitPriceSet {
                shopMoney {
                  amount
                  currencyCode
                }
              }
              discountedUnitPriceSet {
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
                rate
                priceSet {
                  shopMoney {
                    amount
                    currencyCode
                  }
                }
              }
              product {
                id
              }
            }
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
