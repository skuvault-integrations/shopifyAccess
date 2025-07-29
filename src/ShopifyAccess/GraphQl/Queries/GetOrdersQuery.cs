namespace ShopifyAccess.GraphQl.Queries
{
	public static class GetOrdersQuery
	{
		public static readonly string Query = @"query GetOrdersPaginated($query: String!, $first: Int!, $after: String) {
  orders(first: $first, after: $after, query: $query) {
    edges {
      cursor
      node {
        id
        name
        createdAt        
        totalPriceSet {
          shopMoney {
            amount
            currencyCode
          }
        }
        lineItems(first: 100) {
          edges {
            node {
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
            }
          }
        }
        billingAddress {
          name
          address1
          address2
          city
          province
          country
          zip
          phone
        }
        shippingAddress {
          name
          address1
          address2
          city
          province
          country
          zip
          phone
        }
        closedAt
        cancelledAt
        displayFinancialStatus
        displayFulfillmentStatus
        fulfillments(first: 10) {
          displayStatus
          createdAt
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
        }
        discountCodes
        taxLines {
          title
          price
          rate
        }
        refunds {
          id
          createdAt
          note
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
