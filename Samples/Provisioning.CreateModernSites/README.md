# Create "modern" site collections #

### Summary ###
This sample shows how to use SharePointPnP core component to create modern site collections and to apply a PnP Provisioning Template to them.

![The Web UI of the Provisioning sample application](./images/Provisioning.CreateModernSites-Web-UI.png)

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/SharePoint/PnP-Sites-Core) for the needed API operations.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.CreateModernSites | Paolo Pialorsi (**[PiaSys.com](https://piasys.com/)** - [@PaoloPia](https://twitter.com/PaoloPia))

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | Nov 20th 2017 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------
# SCENARIO: Create a modern site using a custom asynchronous application #
This sample demonstrates how to create "modern" SharePoint Online site collections ("modern" Team sites and "modern" Communication sites) using an Office 365 application registered in Azure Active Directory. Moreover, right after the creation of the "modern" site, the solution applies a PnP Provisioning template to the site.

## Solution Architecture ##
The solution has a fully decoupled architecture, which uses a front-end ASP.NET MVC web site for the UI and a couple of background services to create the modern sites. In the following figure you can see an architectural schema of the solution.

![The Web UI of the Provisioning sample application](./images/Provisioning.CreateModernSites.Architecture.png)

For the sake of making a full example, the solution supports both an Azure Function and an Azure WebJob. They are fully interchangeable, and you can use this sample solution to see the differences and make your choice based on your functional requirements. Both the Azure Function and the Azure WebJob use an Azure Blob Storage Queue to store information about the "modern" sites to create. Through this approach the front-end application is fully decoupled from the back-end services, and you can benefit from a fully asynchronous and scalable architecture.
Notice that, for the sake of sharing an architectural pattern, the front-end application embeds and OAuth 2.0 Access Token into the blob storage queue message, to provide to the back-end service (whether it is an Azure Function or an Azure WebJob) the security context of the interactive user creating the site through the web-based UI.

## Solution Deployment ##
In order to deploy the solution you need to:
* Register an application in Azure AD and update the web.config file of the web application (Provisioning.CreateModernSites) in order to target the ClientId, ClientSecret, Domain, TenantId and SPORootSiteUrl of your environment. If you like, you can use Visual Studio to register the application. The application registered in Azure AD has to have the following delegated permissions for SharePoint Online API:
    * Read managed metadata
    * Have full control  of all site collections
* Create an Azure Blob Storage Account (classic) and two Blob Storage Queues in there. One queue will be called "modernsitesazurefunction" and will be used by the Azure Function. Another queue will be called "modernsitesazurewebjob" and will be used by the Azure WebJob. 
* Configure the Azure Blob Storage Account connection string in both the Azure Function settings (Provisioning.CreateModernSites.Function) and  in the Azure WebJob settings (Provisioning.CreateModernSites.WebJob).
* Publish the ASP.NET MVC application (Provisioning.CreateModernSites) onto an Azure App Service, and configure proper settings in the "Application Settings" section of the App Service configuration.
* Publish the Azure Web Job (Provisioning.CreateModernSites.WebJob) within the same Azure App Service used in the  previous step.
* Publish the Azure Function (Provisioning.CreateModernSites.Function) targeting a new Azure Function, which you can create directly from Visual Studio.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.CreateModernSites" />