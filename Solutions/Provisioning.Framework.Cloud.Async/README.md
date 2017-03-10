# PnP Provisioning Engine - Self-service site collection provisioning #

### Summary ###
Solution shows how to build self-service site collection provisioning solution using the Office 365 Developer PnP provisioning engine.

This solution shows following capabilities
- Self service UI to request site collections
- Request are processed asynchronously using Azure storage queues and Azure WebJobs
- New site collection creation to Office 365
- Apply configuration template to existing site using xml based definition
- Apply configuration template which is extracted from existing site during process and applied on top of the newly created site

*Notice that this is just one implementation of core engine, you can take advantage of the engine also using PowerShell.*


### Applies to ###
-  Office 365 Multi-Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises*

Technically engine works within on-premises as well, but Azure pieces are not available there.

### Prerequisites ###
Azure subscription and existing Azure Storage Queue which can be configured for the sites to be used the queuing mechanism for the site collection provisioning requests.

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.Framework.Cloud.Async | Vesa Juvonen

*PnP remote provisioning Core Engine work done by •Erwin van Hunen (Knowit AB), Paolo Pialorsi (PiaSys.com), Bert Jansen (Microsoft), Frank Marasco (Microsoft), Vesa Juvonen (Microsoft) *

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | April 22nd 2015 | Initial version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Conceptual design #
Following picture shows the conceptual design of this solution.

![Logical design of the solution with 5 steps](http://i.imgur.com/3S21w53.png)

1. Business users will modify actual agreed sites using their browser for needed changes, like site columns, content types, list/libraries and branding
2. There could be one or more template sites, which could be for example divided between organizations and each of them could have separate content editors. This obviously depends on the exact business requirements for each customer
3. End users can use self-service site collection user interface to request site collections or the process could be also administrative driven using PowerShell
4. PnP site provisioning engine will extract delta changes compared to out of the box sites from actual live sites
5. New site collections are created using out of the box site definitions, but changes what the business users have applied to those separate template sites are automatically applied to newly created sites

*Notice that this is just one possible process. Exact scenario depends on business requirements. Key point is that we are able to extract modifications from live sites, which we can either storage as template xml files or applied on-fly to newly created sites.*


# Code level approach #
Here's logical design from code perspective.

![Logica design for the code with 4 steps](http://i.imgur.com/jEsw6uB.png)

1. We can extract the delta changes compared to out of the box site definitions by executing *Web.GetProvisioningTemplate()* extension method. This will return us a domain object, which we can optionally also modify in code, if needed
2. You can serialize the domain object to different formats. Main format is PnP provisioning xml, but also JSOM is already supported. PnP provisioning XML uses community standardize schema available from own [repository](https://github.com/OfficeDev/PnP-Provisioning-Schema) under Office Dev in the GitHub
3. You can save or load serialized domain objects using Connectors. When this was written, we supported file system, SharePoint and Azure blob store connectors for loading and saving the information
4. When you have domain object available either by loading it from some location or from live site, you can apply those changes to any site.

*Notice that you could for example have multiple templates from which one is used for corporate branding, one for standardize library information and third one for special configuration for specific template. This would mean that you'd just apply the configuration on top of target site 3 times. Notice also that you could also use this method to move templates or sites cross tenants and environments, since template domain object is not connected to source site. *

# Solution description #
Here's individual projects what are included in the solution and the needed configuration for them. 

![List of projects in Visual Studio solution](http://i.imgur.com/6HgFECj.png)

### Helper.ApplyCustomTemplate ###
Helper project to test the configuration apply logic. You will need to update the add-in ID information in app.config to make this work properly. 

### Helper.CreateSiteCollection ###
Helper project to test create site collection functionality. You can use this project to ensure that you are using right add-in ID and configuration which has permissions to create new site collections.

You will need to update add-in ID information from the app.config to make this one work properly.

### Helper.SendQueueMessage ###
Helper project which can be used to verify that the Azure storage account is correct.

You will need to configure the Azure storage account accordingly.

### Provisioning.Framework.Cloud.Async ###
Actual add-in project, which introduces the provider hosted add-in for the SharePoint.

### Provisioning.Framework.Cloud.Async.Common ###
Business logic component which actually has all the needed code. 

### Provisioning.Framework.Cloud.Async.Job ###
WebJob project which will be deployed to the Azure. Will be responsible of the actual site collection creation and the logic on how to apply configuration/customization on newly created site.

You will need to update Azure storage queue and add-in Id information accordingly in the app.config or directly in the Azure admin UIs.

### Provisioning.Framework.Cloud.AsyncWeb ###
This is the actual web UI for self-service site collection creation. Notice again that this is just a one way of exposing options for end user. You could just as well ask additional metadata or implement the UI using asp.net MVC project.

You will need to update correct Azure storage key for the web.config.

![UI of the site request form](http://i.imgur.com/mmiuWFA.png)

<img src="https://telemetry.sharepointpnp.com/pnp/solutions/Provisioning.Framework.Cloud.Async" />