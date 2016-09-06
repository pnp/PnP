# Microsoft Graph - Query personal files and emails #

### Summary ###
This is simplistic ASP.net MVC application to query personal emails and files using Microsoft Graph showing also dynamic querying of the information with ajax queries. Sample uses also Office UI Fabric to provide consistent user interface experience with standardized controls and presentation.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
App configuration in the Azure AD

### Solution ###
Solution | Author(s)
---------|----------
Office365Api.Graph.Simple.MailAndFiles | Vesa Juvonen

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | February 5th 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# Introduction #
This sample is demonstrating simplistic connectivity to the Microsoft Graph to show emails and files of the particular user. UI will automatically refresh the different parts of the UI, if there's new items arriving to email inbox or added to user's OneDrive for Business site.

![App UI](http://i.imgur.com/Rt4d8Py.png)

# Azure Active Directory Setup #
Before this sample can be executed, you will need to register application to Azure AD and provide needed permissions for the Graph queires to work. We will create an application entry to Azure Active Directory and configure the needed permissions.

- Open up the Azure Portal UI and move to Active Directory UIs - at the time of writing, this is only available in the old portal UIs.
- Move to **applications** selection
- Click **Add** to start the creation of a new app
- Click **Add application my organization is developing**

![What do you want to do UI in Azure AD](http://i.imgur.com/dNtLtnl.png)

- Provide your application a **name** and select **Web Application and Web API** as the type

![Add application UI](http://i.imgur.com/BrxalG7.png)

- Update app properties as follows for debugging
	- **URL** - https://localhost:44301/
	- **APP ID URL** - valid URI like http://pnpemailfiles.contoso.local - this is just an identier, so it does not have to be actual valid URL

![App details UI](http://i.imgur.com/1IaNxLm.png)

- Move to **configure** page and section around keys
- Select 1 or 2 year during for the generated secret

![Secret life cycle setting](http://i.imgur.com/7kX396J.png)

- Click **Save** and copy the generated secret for future usage from the page - notice that the secret is ONLY visible during this time, so you will need to secure that to some other location.

![Client Secret](http://i.imgur.com/5vnkkTA.png)

- Scroll down for the permission configuration

![Permissions to other applications](http://i.imgur.com/tF4R75w.png)

- Select Office 365 Exchange Online and Office 365 SharePoint Online as the applications to which you want to assign permissions

![Permission assigning](http://i.imgur.com/XGOba3Y.png)

- Give "**Read User Mail**" permission under Exchange Online permissions

![Selection of needed permissions for Exchange](http://i.imgur.com/CyH9gg2.png)

- Give "**Read user files**" permission under SharePoint Online permissions

![Selection of needed permissions for SharePoint](http://i.imgur.com/NSZiHsh.png)

- Click **Save** 

You have now completed the the needed configuration at the Azure Active Directory part. Notice that you will need to still configure client id and secret to web.config file in the project. Update the client ID and ClientSecret keys properly.

![Configuration of web.config](http://i.imgur.com/pihBvR5.png)

# Run the solution #
Whenever you have configured the Azure AD side and updated the web.config based on your environmental values, you can run the sample properly.

- Press F5 in the Visual Studio
- Click **Connect to Office 365** or **Sign-in** from the suite bar, which will show the AAD concent UI to sign-in to the right Azure AD

![App UI](http://i.imgur.com/YMCrG4O.png)

- Sign-in with the right Azure Active Directory credentials to the application

![Sign-in to Azure AD - Consent UI](http://i.imgur.com/gNz5Wgz.png)

- You will be shown the UI of the application

![UI of application with your personal data](http://i.imgur.com/Rt4d8Py.png)


<img src="https://telemetry.sharepointpnp.com/pnp/samples/MicrosoftGraph.Office365.Simple.MailAndFiles" />