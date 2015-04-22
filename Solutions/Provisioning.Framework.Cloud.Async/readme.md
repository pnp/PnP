# PnP Provisioning Engine - Self service site collection provisioning #

### Summary ###
Solution shows how to build self-service site collection provisioning solution using the Office 365 Developer PnP provisioning engine.

This solution shows following capabilities
- Self service UI to request site collections
- Request are processed asynchroniously using Azure storage queues and Azure WebJobs
- New site collection creation to Office 365
- Apply configuration template to existing site using xml based definition
- Apply configuration template which is extracted from existing site during process and applied on top of the newly created site

*Notice that this is just one implementation of core engine, you can take advantage of the engine also using PowerShell.*


### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises*

Technically engine works within on-premises as well, but Azure pieces are not available there.

### Prerequisites ###
Azure subscription and existing Azure Storage Queue which can be configured for the sites to be used the queuing mechanism for the site collection provisioning requests.

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.Framework.Cloud.Async | Vesa Juvonen

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | April 22nd 2015 | Initial version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Conceptual design #
Following picture shows the conceptual design of this solution.

![](http://i.imgur.com/3S21w53.png)




# Code level approach #
Description

![](http://i.imgur.com/jEsw6uB.png)



# Solution description #
Here's individual projects what are included in the solution and the needed configuration for them. 

![](http://i.imgur.com/6HgFECj.png)

### Helper.ApplyCustomTemplate ###
Helper project to test the configuration apply logic. You will need to update the App ID information in app.config to make this work properly. 

### Helper.CreateSiteCollection ###
Helper project to test create site collection functionality. You can use this project to ensure that you are using right App ID and configuration which has permissions to create new site collections.

You will need to update App ID information from the app.config to make this one work properly.

### Helper.SendQueueMessage ###
Helper project which can be used to verify that the Azure storage account is correct.

You will need to configure the Azure storage account accordingly.

### Provisioning.Framework.Cloud.Async ###
Actual app project, which introduces the provider hosted app for the SharePoint.

### Provisioning.Framework.Cloud.Async.Common ###
Business logic component which actually has all the needed code. 

### Provisioning.Framework.Cloud.Async.Job ###
WebJob project which will be deployed to the Azure. Will be responsible of the actual site collection creation and the logic on how to apply configuration/customization on newly created site.

You will need to update Azure storage queue and App Id information accordingly in the app.config or directly in the Azure admin UIs.

### Provisioning.Framework.Cloud.AsyncWeb ###
This is the actual web UI for self service site collection creation. Notice again that this is just a one way of exposing options for end user. You could just as well ask additional metadata or implement the UI using asp.net MVC project.

You will need to update correct Azure storage key for the web.config.

![](http://i.imgur.com/mmiuWFA.png)