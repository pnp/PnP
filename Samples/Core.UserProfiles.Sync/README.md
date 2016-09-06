# Azure AD to User Profile Sync Tool #

### Overview ###
This solution builds on top of the UserProfile.Manipulation.CSOM.Console code sample. The business problem that it solves is that not all Azure AD properties are synced to SharePoint User Profiles, so you must cater for your own sync process for any other properties (like extended) of interest.

It aims to show a working solution that:

- you can configure which properties to read and where to write them
- authenticates & pulls data from Azure AD
- authenticates and updates all user profiles with the data from Azure AD. v1 of this sample uses the SharePointOnlineCredentials object.

Key points are that it uses the new API for updating **User Profiles with CSOM** (not the web services) and the use of the new **Graph API 2.0** recently announced here: [http://blogs.msdn.com/b/aadgraphteam/archive/2014/12/12/announcing-azure-ad-graph-api-client-library-2-0.aspx](http://blogs.msdn.com/b/aadgraphteam/archive/2014/12/12/announcing-azure-ad-graph-api-client-library-2-0.aspx)

One of the interesting demonstration elements is giving access to a SharePoint add-in to read (or write) in the Azure AD instance behind an Office 365 tenant.

Many of the concepts are described in other samples and blog posts:

- **Set another user's profile properties with CSOM** ([http://www.vrdmn.com/2014/11/set-another-users-profile-properties.html](http://www.vrdmn.com/2014/11/set-another-users-profile-properties.html)) by Vardhaman Deshpande
- **UserProfile.Manipulation.CSOM.Console** ([https://github.com/OfficeDev/PnP/tree/master/Samples/UserProfile.Manipulation.CSOM.Console](https://github.com/OfficeDev/PnP/tree/master/Samples/UserProfile.Manipulation.CSOM.Console))
- **SharePoint user profile properties now writable with CSOM** ([http://blogs.msdn.com/b/vesku/archive/2014/11/07/sharepoint-user-profile-properties-now-writable-with-csom.aspx](http://blogs.msdn.com/b/vesku/archive/2014/11/07/sharepoint-user-profile-properties-now-writable-with-csom.aspx)) by Vesa Juvonen
- **AzureAD.GroupMembership** ([https://github.com/OfficeDev/PnP/tree/master/Samples/AzureAD.GroupMembership](https://github.com/OfficeDev/PnP/tree/master/Samples/AzureAD.GroupMembership)) (note: this sample uses the older Graph API at the time of writing of v1 of this sample)

Note that there is no SharePoint add-in, just a console application, which you can run as a scheduled task or Azure WebJob.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
- A ClientId and ClientSecret registered with SharePoint's "appregnew.aspx" page
- Credentials to authenticate to SharePoint Online with tenand admin rights to write to user profiles
- The ClientId to have permissions to read Azure AD user data

### Solution ###
Solution | Author(s)
---------|----------
Core.UserProfiles.Sync | Radi Atanassov (OneBit Software) & Teodora Ivanova (OneBit Software)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | January 18th 2015 | Initial release, uses SharePointOnlineCredentials

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------
# Setup and Execution #
## Register ClientId & ClientSecret ##
Given that it is a console app, Visual Studio will not register a ClientId/Secret and put them in the web.config for you. You need to do this yourself.

Start by getting and registering a ClientId and ClientSecret through /_layouts/15/AppRegNew.aspx:
![Registration of ClientId and Secret](http://i.imgur.com/FrgFeht.png)

1. Generate your ClientId & ClientSecret and save them
2. Use a title of your choice
3. Add-in Domain: because this is a console app it doesn't really matter, this will not be used because there is no end user interaction
4. Redirect URI: same as above, I just use localhost, there will be no on-the-fly permissions

## Give your ClientId access to read Azure AD ##
If you have done the above, you have actually registered your add-in (your ClientId) with the Azure AD instance behind Office 365. Although it is registered, it still doesn't have permissions to read the directory. 

Using the Azure AD PowerShell module (get it from [http://msdn.microsoft.com/en-us/library/azure/jj151815.aspx](http://msdn.microsoft.com/en-us/library/azure/jj151815.aspx)) run this script after you plug in your ClientId:

```PowerShell
## Connect to the Microsoft Online tenant
Connect-MsolService

## Set the add-in Client Id, aka AppPrincipalId, in a variable
$appId = "9a329aa2-01de-4248-8dc4-3187ed7e1c6c"

## get the add-in Service Principal
$appPrincipal = Get-MsolServicePrincipal -AppPrincipalId $appId 

## Get the Directory Readers Role
$directoryReaderRole = Get-MsolRole -RoleName "Directory Readers" ##get the role you want to set

##Give the add-in the Directory Reader role
Add-MsolRoleMember -RoleMemberType ServicePrincipal -RoleObjectId $directoryReaderRole.ObjectId -RoleMemberObjectId $appPrincipal.ObjectId

##Confirm that the role has our add-in
Get-MsolRoleMember -RoleObjectId $directoryReaderRole.ObjectId
```

This PowerShell script gets the Service Principal of the add-in, then gives it the "Directory Readers" permission. This is enough to get the users and their data.

## Configure the fields to synchronize ##
You will find PropertyConfiguration.xml in the project. This is a simple XML file that maps the Azure AD profile property schema name and the User Profiles property name. It also specifies the WriteIfBlank behaviour, and if the field is multi-value.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Configuration>
  <Properties>
    <Property ADAttributeName="Country" UserProfileAttributeName="Test-Country" WriteIfBlank="true" IsMulti="false"/>
  </Properties>
</Configuration>
```
## Settings in the App.Config file ##
Some key settings are required in the app.config file. 

This solution still makes use of SharePointOnlineCredentials, so think about how to protect the password (encrypt it, etc).

Note: I didn't manage to get this working with an Add-in Only Policy context... the PeopleManager doesn't seem to work without a user context.
```XML
  <appSettings>
    <!--Used in SharePoint and in Azure AD-->
    <add key="ClientId" value="GUID HERE" />
    <add key="ClientSecret" value="SECRET HERE" />
    
    <!--Used in Azure AD AuthenticationHelper-->

    <add key="TenantSharePointAdminUrl" value="https://tenantid-admin.sharepoint.com" /> <!--Used in console code for Admin url-->
    <add key="TenantUpnDomain" value="tenantid.onmicrosoft.com" />
  
    <!--Used to buils SharePointOnlineCredentials to write to UPA-->
    <add key="TenantAdminLogin" value="admin@tenantid.onmicrosoft.com" />
    <add key="TenantAdminPassword" value="PASSWORD HERE" /> 
  
    <!--Not used, but you could save the extra lookup of TokenHelper.GetRealmFromTargetUrl(sharePointAdminUrl); realm is tenantid --> 
    <!--<add key="TenantId" value="GUID HERE" /> left for academic reasons-->
  </appSettings>
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.UserProfile.Sync" />