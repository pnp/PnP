# Using the Azure AD Graph API to check group membership #

### Summary ###
This sample shows how you can use the Azure AD Graph API to check if a given user is member of a group. 

You can find a few other useful samples in the code:
- how to get all groups
- how to get all users
- how to list all groups that a user belongs to

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
Option 1:
-  Azure subscription with Azure AD setup
-  Registration of an app in Azure AD is needed to make this sample work

Option 2:
-  Registering your ClientId and ClientSecret with SPO appregnew.aspx
-  Giving permission through the Windows PowerShell Azure AD Module

### Solution ###
Solution | Author(s)
---------|----------
AzureAD.GroupMembership | Bert Jansen (**Microsoft**)
AzureAD.GroupMembership | Radi Atanassov (**OneBit Software**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | October 7th 2014 | Initial release
2.0  | January 19th 2015 | Updated to Graph API 2.0; Updated code with more samples; Added documentation option without an Azure subscription.

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Use case description #
The use case for this sample is showing how you can leverage your AD information, even when your app is running in Microsoft Azure. Once you're running in Microsoft Azure there's no direct connection anymore to your local on-premises Active Directory which prevents scenarios like checking if a user is member of an AD group. Luckily most SharePoint Online customers do sync their on-premises Active Directory with Azure AD...which gives us the opportunity to query Azure AD instead of the on-premises Active Directory. Below diagram shows all components together:

![Logical Design of the solution](http://i.imgur.com/tPzGRCC.png)

An example use to leverage the Azure AD could be a provisioning app that depending on the group membership of a user provisions a site collection using additional elements.

## Transitive vs Intransitive operations ##
When using Group Membership to authorize operations, be aware of differing behaviors of the MemberOf and CheckMemberGroup operations. 
> The operation is intransitive, that is, it will only return groups that the object is a direct member of. This is unlike the isMemberOf function of the directory service, which is transitive and which will return true if the object being tested is a member of a group either directly or through the object's membership in another group.
> 
> [MSDN](https://msdn.microsoft.com/en-us/library/azure/dn151667.aspx)

Given the scenario:
 
Group Name | Members
-----------|---------
East | Jill
West | Jack
All  | East, West

Calling the CheckMemberGroups operation on the user "Jill" will return True despite the fact that calling the MemberOf operation on the user "Jill" will return only the group "East" and not group "All."

# Configuration Options #
Version 1.0 (and setup Option 1) was released with a configuration option that uses the Azure Management Portal to register the Application ID. You need an Azure subscription for the management portal - something you do not get with Office 365. (You can always register a trial subscription if you haven't already.)

While version 2.0 updates the codebase to use the Graph API 2.0, the setup for Option 2 illustrates an alternative approach to register your application with Azure AD. Instead of using the Azure Management Portal, SPO's appregnew.aspx page is used and you **do not** need an Azure subscription.


# Setup (Option 1) #
Before you can use this sampe you'll first need to do a number of setup tasks.

## Ensure you've an Azure AD ##
I assume that the people using this sample also have an Office 365 tenant...and as such they also have an Azure AD because that's what Office 365 uses to store its users and groups. You'll need to either ensure you can login to the Azure tenant that holds this Azure AD or associate the Office 365 Azure AD with an existing Azure tenant (http://technet.microsoft.com/en-us/library/dn629580.aspx). High level the below steps are needed:
- log into the Azure subscription normally (using Azure admin credentails) then
- In the Management Portal go to Windows Azure Active directory section and click the +  to create the new AAD tenant (New->Directory->Custom Create). Note that there is now a dropdown that says by default "Create new directory" but you can change that to "Use existing directory". This allows you to fill details of your existing AAD tenant
- Just follow the wizard (you will be asked for your O365 AAD tenant Admin credentials).
- When you log back (using your Azure admin credentials) to the Azure Management Portal you should see your Office 365 AAD tenant

A more detailed step by step instruction set can also be found at http://www.edutech.me.uk/active-directory/link-microsoft-office-365-organization-account-to-windows-azure-subscription.

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
    <add key="TenantUpnName" value="<tenantname>.onmicrosoft.com"/>
    <add key="ClientId" value="<client id>"/>
    <add key="ClientSecret" value="<client secret>"/>
  </appSettings>
```

# Setup (Option 2) #
Option 1 and Option 2 achieve the same thing - you register an App with Azure AD. 
- you need to do this so Azure AD can authenticate your app with OAuth (instead of user names and passwords)
- Option 2 doesn't use the Azure Management Portal, it shows how you can do this without an Azure subscription (just using Office 365 and whatever you get with it)

This is valid when you (or your client) just has Office 365 and no Azure.

## Register ClientId & ClientSecret ##
Given that the sample is a console app, Visual Studio will not register a ClientId/Secret and put them in the web.config for you.
You need to do this yourself. In a production environment you don't even have Visual Studio, this is how you would deploy this in production.

Start by getting and registering a ClientId and ClientSecret through /_layouts/15/AppRegNew.aspx:

![Registration of teh client id and secret in appregnew.aspx](http://i.imgur.com/IznipPJ.png)

Put a useful 'Title'. I use "localhost" for the App Domain and "https://localhost" for the Redirect UI. In the case of a console app you don't need niether of those.

When you click 'Create', this form will do some magic - it will register a service principal with Azure AD (check it with Get-MsolServicePrincipal).

Make sure you save/copy the values so you can plug them in the web.config.

## Give your ClientId access to read Azure AD ##
If you have done the above, you have actually registered your app (your ClientId) with the Azure AD instance behind Office 365. Although it is registered, it still doesn't have permissions to read the directory. 

Using the Azure AD PowerShell module (get it from [http://msdn.microsoft.com/en-us/library/azure/jj151815.aspx](http://msdn.microsoft.com/en-us/library/azure/jj151815.aspx)) run this script after you plug in your ClientId:

```PowerShell
## Connect to the Microsoft Online tenant
Connect-MsolService

## Set the app Client Id, aka AppPrincipalId, in a variable
$appId = "9a329aa2-01de-4248-8dc4-3187ed7e1c6c"

## get the App Service Principal
$appPrincipal = Get-MsolServicePrincipal -AppPrincipalId $appId 

## Get the Directory Readers Role
$directoryReaderRole = Get-MsolRole -RoleName "Directory Readers" ##get the role you want to set

##Give the app the Directory Reader role
Add-MsolRoleMember -RoleMemberType ServicePrincipal -RoleObjectId $directoryReaderRole.ObjectId -RoleMemberObjectId $appPrincipal.ObjectId

##Confirm that the role has our app
Get-MsolRoleMember -RoleObjectId $directoryReaderRole.ObjectId
```

This PowerShell script gets the Service Principal of the app, then gives it the "Directory Readers" permission. This is enough to get the users and their data.

## Update the app.config file ##
Update the app.config file with the client ID and secret you've obtained in the "appregnew.aspx" step & specify your tenant upn address.

```XML
  <appSettings>
    <add key="TenantUpnName" value="<tenantname>.onmicrosoft.com"/>
    <add key="ClientId" value="<client id>"/>
    <add key="ClientSecret" value="<client secret>"/>
  </appSettings>
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/AzureAD.GroupMembership" />