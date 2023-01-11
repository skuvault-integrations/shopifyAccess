Here is the sequence diagram of Shopify OAuth authorization flow from SkuVault Channel Accounts to Shopfy (and back again):
```mermaid
sequenceDiagram
    actor Tenant
    Tenant->>Shopify Channel Account js / html: Creates Shopify channel account
    Shopify Channel Account js / html->>Channel Accounts Controller: GetShopifyAppInstallationUrl()
    Channel Accounts Controller-->>Shopify Channel Account js / html: Uri
    Shopify Channel Account js / html->>Shopify App Authorization Page: Redirect to Shopify app authorization Url (popup)
    Shopify Channel Account js / html->>Shopify Channel Account js / html: Poll for account creation
    Note over Shopify App Authorization Page: User installs the SV app in their Shopify store
    Shopify App Authorization Page->>Channel Accounts Controller: Call Shopify app redirect Url ReceiveShopifyCodeV2 with authorization code, shop Url, state
    Channel Accounts Controller->>Shopify API: Get permanent access token for this shop
    Shopify API-->>Channel Accounts Controller: Access token
    Note over Channel Accounts Controller: Save the returned access token into channel account for this Shopify shop
    Shopify Channel Account js / html->>Tenant: Display that account has been connected to Shopify
```
