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
        }
        closedAt
        cancelledAt
        displayFinancialStatus
        displayFulfillmentStatus
        fulfillments(first: 10) {
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
          fulfillmentLineItems(first: 10) {
            edges {
              node {
                lineItem {
                  id
                  sku
                  quantity
                  product {
                    variants(first: 10) {
                      edges {
                        node {
                          inventoryItem {
                            measurement {
                              weight {
                                unit
                                value
                              }
                            }
                          }
                        }
                      }
                    }
                  }
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
          refundLineItems {
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
