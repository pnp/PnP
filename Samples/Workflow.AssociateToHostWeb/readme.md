# Workflow.AssociateToHostWeb #

### Summary ###
This sample demonstrates a technique to show how to associate an app deployed integrated workflow to the host web. 

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

**NOTE:** This is only an interim solution


### Solution ###
Solution | Author(s)
---------|----------
Workflow.AssociateToHostWeb | Tim McConnell (Microsoft), Brian Michely (Microsoft), Frank Marasco (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | 7/28/2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# GENERAL COMMENTS #
Integrated workflow apps provide the capability to associate a workflow deployed in an app to a list in the host/parent web.
This is only an interim solution until the VS tooling has been updated to perform this activity. 


## PREPARING THE .APP FILE FOR DEPLOYMENT ##

Here are the basic steps to create your app, add workflows, publish the .app file and make necessary modifications that allow you to associate your workflow(s) to the host web:

- Create your SharePoint hosted app and then create/add your workflow(s) to the app project 
- Build and publish the app. 
- Once your .app file has been created, rename the extension to .zip
- Extract the contents
- Edit the WorkflowManifest.xml (which will be empty) and add the following:	

```XML
<SPIntegratedWorkflow xmlns="http://schemas.microsoft.com/sharepoint/2014/app/integratedworkflow">
    <IntegratedApp>true</IntegratedApp>
</SPIntegratedWorkflow>
```
-  Once this is done save your file, and package the app again select all the files inside the extracted folder -> right click -> Send to -> Compressed (zipped) folder.
-  Rename the .zip file back to .app


## PREPARING THE .APP FILE FOR DEPLOYMENT ##
	
Once the .app file has been modified to include the WorkflowManifest.xml change:

- Install the app 
- Go to workflow settings -> Add a workflow in the list to which workflow has to be associated. 
- Now you will see App Selection options similar to this:

![](http://i.imgur.com/tUADxZ9.png)

- Select your workflow app and continue as you would for normal workflow association
- Trigger a new workflow either by a manual start process and selecting the workflow app or other preferred start options.


## DEPENDENCIES ##
- Microsoft.SharePoint.Client.dll
- Microsoft.SharePoint.Client.Runtime.dll