# Provision sites in batches with the add-in model #

### Summary ###
This sample demonstrates how to create a console add-in that provisions site collections by using the CSOM and the add-in model.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises


### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.Batch | Jim Crowley (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | January 25th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Key components of the sample #

The sample add-in contains the following:

- **Batch Provisioning project**, which contains the AppManifest.xml file. This file registers the provider-hosted add-in with SharePoint.
- **Batch Provisioning Console project**, which contains:
    - **App.config fil**e: Contains the client id and secret for the add-in
    - **Sites.xml**: Contains a list of the names of the site collections that the add-in will create
    - **Program.cs**: Contains the code that provisions the site collections listed in Sites.xml
    - **TokenHelper.cs**: Contains the helper code that enables the add-in to get the required permissions for creating site collections. This file is not a default component of the console application template in Visual Studio 2013. You can get this file by creating a provider-hosted or autohosted add-in for SharePoint and copying the file from the remote web application to the console app project.
- **Batch ProvisioningWeb project**, which contains the TokenHelper.cs file and also the default components of a provider-hosted add-in. You can deploy this web application to a Windows Azure site to make sure that your add-in's credentials are working.

# Configure the sample #
Follow these steps to configure the sample.

1. Open the Batch Provisioning.sln file in Visual Studio 2012.
2. In the Properties pane, change the Site URL property. It is the absolute URL of your SharePoint test site collection on Office 365: https://<my tenant>.sharepoint.com/.


# Build, deploy, and run the sample #
## To build and deploy the Batch Provisioning web application ##

1. Create an empty website on Windows Azure and download the publishing profile for that site.
2. Register an add-in at the /_layouts/15/appregnew.aspx page of your SharePoint test site collection on Office 365: https://<my tenant>.sharepoint.com/_layouts/15/appregnew.aspx. Be sure to fill in the following details:

    - Generate a client ID and client secret. You'll need to add both values to the  web.config and app.config files in the solution. You'll also need to provide the client secret to the publishing wizard.
    - Enter the URL of the website that you created on Windows Azure for App Domain.
    - Leave the Redirect URI field empty.

3. Open the App.config file in the Batch Provisioning Console project and add values for the ClientId and ClientSecret keys. Use the client id and client secret values that you created when you registered the add-in.
4. Open the Web.config file in the Batch ProvisioningWeb project and add values for the ClientId and ClientSecret keys. These values are the same as the ones that you add to the App.config file.
5. Open the Program.cs file in the Batch Provisioning Console project. Add values for these variables:

    - sharePointUrl : This must be the URL of the admin site of your Office 365 tenant. You can find this URL by going to the home page of your Office 365 SharePoint site, clicking the Admin menu, and selecting SharePoint.
    - path : The file system path that points to the location of your Sites.xml file.
    - Owner parameter of newSite: The email address of the administrator of your Office 365 tenant. This user must be a tenant administrator.

6. Open the Sites.xml file in the Batch Provisioning Console project. Add at least one or more URLs for the site or sites that you want to create in your Office 365 tenant.
7. Right-click the Batch ProvisioningWeb project in Solution Explorer, and then select Publishing.
8. Follow the instructions to import the publishing profile of your Windows Azure site, and publish the project to Windows Azure.


## To build and deploy the add-in for SharePoint ##

1. Right-click the Batch Provisioning project in Solution Explorer, and then select Publish.
2. For Which profile do you want to publish, type Batch Provisioning to create a publishing profile. Click Next.
3. For Where is your website hosted, type the location of the Windows Azure site where you published the Batch ProvisioningWeb project.
4. For client ID, type the client ID value that you created when you registered the add-in.
5. For client secret, type the client secret value that you created when you registered the add-in.
6. Click Next, and then click Finish. The resulting add-in package file has an .app extension (Batch Provisioning.app) and is saved in the app.publish subfolder of the bin\Debug folder of the Visual Studio solution.
7. In your browser, navigate to your Office 365 site. Click the Admin drop-down list in the upper right corner of the page and select SharePoint to go to the SharePoint admin center.
8. Click apps in the left panel, and then click add-in Catalog in the center column. If you don't have an add-in Catalog site, you'll have to follow the instructions to create a new one.
9. Upload the Batch Provisioning.app file that you created when you published the Batch Provisioning project to the add-in Catalog by following these steps:

    1. Click Apps for SharePoint in the left panel.
    2. Click Files, Upload Document in the ribbon and browse to the Batch Provisioning.app file. You don't have to add any metadata or change any of the default values. 
    3. Click OK.
10. In your browser, navigate to the site collection in your Office 365 where you want to deploy the add-in.
11. Click the gear icon in the top right of the page, and select Add an add-in from the drop-down menu.
12. You'll see a new add-in named Batch Provisioning. Click the name, and then click Trust It.

Wait for the add-in to install.

## To run the sample ##

1. After the add-in installs completely, click the add-in icon to launch the add-in.
2. Verify that the site name appears on your Windows Azure site. This verifies that your add-in's credentials are working.
3. Right-click the Batch Provisioning Console project in Solution Explorer. Select Start new instance from the Debug menu. While the console app is running, go to your Office 365 site, click on the Admin menu, and select SharePoint. The Site Collections list will display your new site or sites and note that they are being provisioned.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.Batch" />