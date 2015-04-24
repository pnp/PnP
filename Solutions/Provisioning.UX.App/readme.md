# PnP Provisioning Engine - Self service site collection provisioning #

### Summary ###
Solution shows a reference sample on how to build self-service site collection provisioning solution using the Office 365 Developer PnP provisioning engine.

This solution shows following capabilities
- Self service UI to request site collections
- Request are processed asynchronously using the remote timer job pattern
- New site collection creation to Office 365
- New site collection creation in SharePoint on-premises build
- Apply a configuration template to existing site using xml based definition


**NOTICE THIS SOLUTION IS STILL UNDER ACTIVE DEVELOPMENT**


### Applies to ###
-  Office 365 Multi-tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises


### Solution ###
Solution | Author(s)
---------|----------
Provisioning.UX.App | Frank Marasco & Brian Michely

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | April 22nd 2015 | Initial version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Conceptual design #
DOCUMENTATION IN PROGRESS

# Solution description #
Projects what are included in the solution and the needed configuration for them. 

### Provisioning.UX.App###
SharePoint Application 

### Provisioning.Common ###
Reusable component  

### Provisioning.Job ###
WebJob project which will be deployed to the Azure Will be responsible of the actual site collection creation and the logic on how to apply configuration/customization on newly created site.

You will need to update App ID/Secret in the app.config

### Provisioning.UX.AppWeb ###
This is the user interface (UX) for self service site collection creation. 

You will need to update App ID/Secret information in the web.config

