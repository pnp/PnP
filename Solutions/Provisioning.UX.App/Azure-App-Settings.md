# Azure App Settings for Provisioning.UX.App
How to configure Provisioning.UX.App using Azure Web Sites App Settings

 **Last modified:** September 2, 2015

 _**Verified with:** PnP Provisioning.UX.App, SharePoint Online, Azure Web Sites_

 
## Configure all settings in Provisioning.UX.App
To avoid working with .config files the Provisioning.UX.App allows you to work with only Azure Web App Settings (or IIS settings). 

In web.config or app.config you do not need to specify any AppSettings such as the client id or client secret or urls, as shown in the image below:
![Azure App Settings](http://i.imgur.com/uRwPFWC.png)

The following settings can be used:

 Setting | Description
-------------------|----------
AutoApproveSites | Used to set the site request to a Approved or New Status to support custom workflows to approve site requests. Set either to true or false
ClientId | Your Client ID 
ClientSecret | Your Client Secret
SPHost | The Site Url that hosts your SharePoint Add-in
SupportTeamNotificationEmail | Used to send notifications if there is an exception. This is reserved for future use in the Web Project
TenantAdminUrl | The Tenant Admin Site Url where the add-in is hosted
HostedAppHostNameOverride | The DNS name where the Web is hosted
EmailFailureSiteTemplate | Template of the failure e-mail
EmailNewSiteTemplate | Template of the new site e-mail
RepositoryManager_connectionString | connectionString value for the RepositoryManager Module
MasterTemplateProvider_connectionString | connectionString value for the MasterTemplateProvider Module
ProvisioningProviders_connectionString | connectionString value for the ProvisioningProvider Module
ProvisioningConnectors_connectionString | connectionString value for the ProvisioningConnector Module