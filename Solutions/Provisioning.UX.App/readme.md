# PnP Provisioning - Self service site collection provisioning reference implementation#

### Summary ###
Solution shows a reference sample on how to build self-service site collection provisioning solution using the Office 365 Developer PnP provisioning engine.

This solution shows following capabilities
- User Interface to request site collections
- Site Requests stored in a SharePoint List
- Request are processed asynchronously using the remote timer job pattern
- New site collection creation to Office 365
- New site collection creation in SharePoint on-premises builds
- Apply a configuration template to existing site using xml based definitions


**NOTICE THIS SOLUTION IS UNDER ACTIVE DEVELOPMENT**


### Applies to ###
-  Office 365 Multi-tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises


### Solution ###
Solution | Author(s)
---------|----------
Provisioning.UX.App | Frank Marasco, Brian Michely and Steven Follis

### Version history ###
Version  | Date | Comments
---------| -----| --------
.1  | April 22nd 2015 | Initial version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Conceptual design #
DOCUMENTATION IN PROGRESS

# Solution description #
Projects what are included in the solution and the needed configuration for them. 

### Provisioning.UX.App###
SharePoint Add-In 

### Provisioning.Common ###
Reusable component for that implements the site provisioning logic

### Provisioning.Job ###
Remote Timer job project which maybe deployed to Azure.  Will be responsible of the actual site collection creation and the logic on how to apply configuration/customization on newly created site.

You will need to update App ID/Secret in the app.config

### Provisioning.UX.AppWeb ###
This is the user interface (UX) for self service site collection creation. This interface was built using primarily AngularJS and HTML. The intent was to create a modern interface that was easy to edit, and extend.

The interface is launched from default.aspx and the wizard itself is modal based and loads HTML views. These views make a wizard provisioning approach that collects data from the user and submits that data to the back-end provisioning engine. 

Landing Page:

![](http://i.imgur.com/TYiBokL.png)

Clicking the "Get Started" button above launches the Wizard:

![](http://i.imgur.com/Jcy7tEF.png)

#### Navigation ####
The wizard can be navigated either via left side navigation or arrow based navigation on the bottom right. The navigation and views are defined in the wizard.modal.html file. Note - next release will most likely load this from a configuration source, but for now, it's a simple modification to the html file to edit your navigation.

![](http://i.imgur.com/uYwJ0ac.png)

#### Services ####
There are some services exposed that can be used to get template and other data from the back-end, and a service for submitting that data. For PnP sample purposes, the the reference data for the sample meta-data fields gets loaded from .json files. There is a **BusinessMetadata factory** that loads the data from the json files and is invoked from the **wizard.modal.controller** script and the HTML fields bind to the model and the data is loaded via a repeater in most cases. This is only for sample purposes and for a real implementation this data may be list driven or from some other source and can be retrieved via other appropriate methods

![](http://i.imgur.com/9hkCeFf.png)

These services use the CSOM controller **provisioning.controller.cs** which uses **OfficeDevPnP.Core.WebAPI**.

#### People Picker ####
This solution also leverages the PnP JSOM version of the PeoplePicker. 

![](http://i.imgur.com/lmbNL2K.png)

#### Site Availability Checking ####
The site details view contains a field where the user specifies the url of their new site. The solution implements an angular directive that fires off and calls the sitequeryservice.js script which does the site availability check. If the site is available, the solution will set the field to validated, and if the site is not available, there will be a message displayed stating this.

#### Confirmation ####
Once user is done with the views in the wizard, they will be presented with a confirmation view and the chance to change their inputs. Once they click the checkmark icon, the site request object data will be submitted to the engine. 

#### Coming Updates ####
We are currently working an update to this interface which uses an angular schema form approach and will allow you to define a schema in json and the fields you wish to use. You can then use one line of html to load your form/view which will then be schema driven and defined there and not in your views.


You will need to update App ID/Secret information in the web.config

