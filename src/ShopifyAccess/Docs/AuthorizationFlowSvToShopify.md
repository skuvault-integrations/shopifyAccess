Here is the sequence diagram of Shopify OAuth authorization flow from SkuVault Channel Accounts to Shopfy (and back again):
```mermaid
sequenceDiagram
    actor Tenant
    participant ChanAcctClient as Shopify Channel Account js / html
    Tenant->>ChanAcctClient: Creates Shopify channel account
    participant ChanAcctCtrl as SkuVault Channel Accounts Controller
    ChanAcctClient->>ChanAcctCtrl: GetShopifyAppInstallationUrl()
    participant ShopifyAppAuth as Shopify App Authorization Page
    ChanAcctCtrl-->>ChanAcctClient: Uri
    ChanAcctClient->>ShopifyAppAuth: Redirect to Shopify app authorization Url
    Note over ShopifyAppAuth: User installs the SV app in their Shopify store
    ShopifyAppAuth->>ChanAcctCtrl: Call Shopify app redirect Url ReceiveShopifyCodeV2 with authorization code, shop Url, state
    Note over ChanAcctCtrl: Validate the returned state
    ChanAcctCtrl->>Shopify API: Get permanent access token for this shop
    Shopify API-->>ChanAcctCtrl: Access token
    Note over ChanAcctCtrl: Save the returned access token into channel account for this Shopify shop
    ChanAcctCtrl->>ChanAcctClient: Reload the channel accounts page
    ChanAcctClient->>Tenant: Display in channel account that account has been connected to Shopify
```
