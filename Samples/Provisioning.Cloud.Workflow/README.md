# Use workflow to provision a SharePoint site (host web) #

### Summary ###
Learn how to use a workflow to provision a SharePoint site by using the remote provisioning pattern and CSOM

### Applies to ###
-  Office 365 Multi Tenant (MT) - Waiting for new re-distributable
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
- Visual Studio 2012 or Visual Studio 2013
- Microsoft Office Developer Tools for Visual Studio
- A SharePoint 2013 development environment
- Create on the host web an approval workflow and associated list identical to the ones deployed to the add-in web

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.Cloud.Workflow | Jim Crowley (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | February 26, 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #
This sample demonstrates how to use a workflow to provision a SharePoint site by using the remote provisioning pattern and CSOM. The sample uses an add-in installed event to associate a remote event receiver with the custom list on the host web.

For a more detailed description of this sample, see Module 5 of the [SharePoint 2013 and SharePoint Online solution pack for branding and site provisioning](http://www.microsoft.com/downloads/details.aspx?FamilyID=2c8011b0-441a-4bab-bf19-6b1009c7d8dd).


## Configure the sample ##
Check the **Handle Add-in Installed** property in the project property of the add-in for SharePoint project.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.Cloud.Workflow" />