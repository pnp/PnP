# Using the Azure AD Graph API to check group membership #

### Summary ###
This sample shows how you can use the Azure AD Graph API to check if a given user is member of a group.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises


### Prerequisites ###
-  Azures subscription with Azure AD setup
-  Registration of an app in Azure AD is needed to make this sample work

### Solution ###
Solution | Author(s)
---------|----------
AzureAD.GroupMembership | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
2.0  | October 7th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Use case description #
The use case for this sample is showing how you can leverage your AD information, even when your app is running in Microsoft Azure. Once you're running in Microsoft Azure there's no direct connection anymore to your local on-premises Active Directory which prevents scenarios like checking if a user is member of an AD group. Luckily most SharePoint Online customers do sync their on-premises Active Directory with Azure AD...which gives us the opportunity to query Azure AD instead of the on-premises Active Directory. Below diagram shows all components together:

![](http://i.imgur.com/tPzGRCC.png)

An example use to leverage the Azure AD could be a provisioning app that depending on the group membership of a user provisions a site collection using additional elements.

# Setup #
Before you can use this sampe you'll first need to do a number of setup tasks.

## Ensure you've an Azure AD ##
I assume that the people using this sample also have an Office 365 tenant...and as such they also have an Azure AD because that's what Office 365 uses to store its users and groups. You'll need to either ensure you can login to the Azure tenant that holds this Azure AD or associate the Office 365 Azure AD with an existing Azure tenant (http://technet.microsoft.com/en-us/library/dn629580.aspx). High level the below steps are needed:
- log into the Azure subscription normally (using Azure admin credentails) then
- In the Management Portal go to Windows Azure Active directory section and click the +  to create the new AAD tenant (New->Directory->Custom Create). Note that there is now a dropdown that says by default "Create new directory" but you can change that to "Use existing directory". This allows you to fill details of your existing AAD tenant
- Just follow the wizard (you will be asked for your O365 AAD tenant Admin credentials).
- When you log back (using your Azure admin credentials) to the Azure Management Portal you should see your Office 365 AAD tenant

## Register an Azure AD App ##
Next step is registering an app that has read permissions to the Azure AD. You'll need to clientid and clientsecret of this app in order to run this sample against your Azure AD. Follow below steps to register an Azure app:
- Sign in to the Azure management portal
- Click on Active Directory in the left hand nav
- Click the directory tenant where you wish to register the sample application
- Click the Applications tab
- In the drawer, click Add
- Click "Add an application my organization is developing"
- Enter a friendly name for the application, for example "Read Azure AD from SharePoint apps", select "Web Application and/or Web API", and click next
8. For the Sign-on URL, enter a value (this is not used for the console app, so is only needed for this initial configuration): "http://localhost"
- For the App ID URI, enter "http://localhost". Click the checkmark to complete the initial configuration
- While still in the Azure portal, click the Configure tab of your application
- Find the Client ID value and copy it aside, you will need this later when configuring your application
- Under the Keys section, select either a 1year or 2year key - the keyValue will be displayed after you save the configuration at the end - it will be displayed, and you should save this to a secure location. **Note, that the key value is only displayed once, and you will not be able to retrieve it later**
- Configure Permissions - under the "Permissions to other applications" section, you will configure permissions to access the Graph (Windows Azure Active Directory). For "Windows Azure Active Directory" under the first permission column (Application Permission:1"), select "Read directory data". Notes: this configures the App to use OAuth Client Credentials, and have Read access permissions for the application
- Select the Save button at the bottom of the screen - **upon successful configuration, your Key value should now be displayed - please copy and store this value in a secure location**

## Update the app.config file ##
Update the app.config file with the client ID and secret you've obtained in the previous step + specify your tenant name.

```XML
  <appSettings>
    <add key="TenantName" value="<tenantname>.onmicrosoft.com"/>
    <add key="AzureADClientId" value="<client id>"/>
    <add key="AzureADClientSecret" value="<client secret>"/>
  </appSettings>
```

