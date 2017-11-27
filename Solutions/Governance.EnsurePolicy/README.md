# Ensuring access policy for sites (alternative for web app policies) #

### Summary ###

This sample application is a console application that can either be run as a scheduled task or as a web job in Microsoft Azure and which has as goal to grant a user or group permissions to one or more site collections in SharePoint Online. This application can be used an alternative for the “grant” feature of SharePoint Web Application policies. Checkout the [Alternative model for web app policies in SharePoint Online](https://docs.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-webapppolicies) article to learn more.

### Pre-Requisites ###
Before you can run this application, you need to take in account the following pre-requisites:
 - Use SharePoint Online: this application does not work for SharePoint On-premises builds (so Legacy D), it requires SharePoint Online (DvNext or MT)
 - The application uses an Azure AD application for app-only access to SharePoint Online

### Applies to ###
-  Office 365 Multi-tenant (MT)

### Solution ###
Solution | Author(s)
---------|----------
Governance.EnsurePolicy  | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | November 10, 2017 | Initial version

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

**NOTICE THIS SOLUTION IS UNDER ACTIVE DEVELOPMENT**

## Configuring the application
### Defining “how” you want to grant permissions to your SharePoint sites
The sample application uses a PnP Provisioning template to define how the permissions to a site need to be configured which allows you to use one or more of the below scenarios. Below chapters describe the support scenarios but before we can jump into that let’s explain how a user or group must be encoded. Users are simple as you can simply use the user’s user principal name (UPN) value which often is the same as the email address or SIP address of the user, groups however are more complex and require you to enter the group’s SID. The easiest model to do this is by adding that needed group into an existing SharePoint group (e.g. your site’s Members group) and the clicking on the name. This will show you the needed information:

![People and groups](https://i.imgur.com/ipKYR3p.png)

In the below chapter we work with the following samples:
 - **User**: kevinc@bertonline.onmicrosoft.com
 - **Group**: c:0-.f|rolemanager|s-1-5-21-992416943-3518085199-3295385949-83050576

Having clarified how to encode a user and group we can now dive into the possible PnP Provisioning XML options. Below chapters give a sample of each approach, but you can also combine approaches like granting an account site collection admin + creating a new SharePoint group with Read permissions in which you store 5 users.

> **NOTE:**
> The sample contains an permissions.xml file showing below options. You can copy that one and modify it for your own needs.

#### Grant site collection administrator permissions
Below sample shows how you can add a user or a group to the site collection administrators:

```XML
<pnp:ProvisioningTemplate ID="security_add" Version="1" xmlns:pnp="http://schemas.dev.office.com/PnP/2017/05/ProvisioningSchema">
  <pnp:Security>
    <!-- Shows how to add users and groups to the site collection administrators -->
    <pnp:AdditionalAdministrators>
      <pnp:User Name="kevinc@bertonline.onmicrosoft.com"/>
      <pnp:User Name="c:0-.f|rolemanager|s-1-5-21-992416943-3518085199-3295385949-83050576"/>
    </pnp:AdditionalAdministrators>
  </pnp:Security>
</pnp:ProvisioningTemplate>
```

#### Grant permissions by adding a user/group to an existing SharePoint group
This sample shows how you can add a user or group into the OOB SharePoint Visitors group. You can also do the same for the SharePoint members and owners groups by using the AdditionalOwners and AdditionalMembers elements.

```XML
<pnp:ProvisioningTemplate ID="security_add" Version="1" xmlns:pnp="http://schemas.dev.office.com/PnP/2017/05/ProvisioningSchema">
  <pnp:Security>
    <!-- Shows how to add users and groups to the existing site visitors group -->
    <pnp:AdditionalVisitors>
      <pnp:User Name="kevinc@bertonline.onmicrosoft.com"/>
      <pnp:User Name="c:0-.f|rolemanager|s-1-5-21-992416943-3518085199-3295385949-83050576"/>
    </pnp:AdditionalVisitors>
  </pnp:Security>
</pnp:ProvisioningTemplate>
```

#### Grant permissions by adding a user/group into a newly created SharePoint Group
You can also create a new SharePoint group (ContosoMandatoryPermissions in this case), add users and/or groups and then grant that group a permission level (read in this case). You can for example also grant “Full Control” or “Contribute” as permission level.

```XML
<pnp:ProvisioningTemplate ID="security_add" Version="1" xmlns:pnp="http://schemas.dev.office.com/PnP/2017/05/ProvisioningSchema">
  <pnp:Security>
    <!-- Shows how to add a custom SharePoint group, add user and group and grant the OOB read role -->
    <pnp:SiteGroups>
      <pnp:SiteGroup Title="ContosoMandatoryPermissions" 
                     AllowRequestToJoinLeave="false"  
                     AutoAcceptRequestToJoinLeave="false"
                     OnlyAllowMembersViewMembership="true"
                     Owner="{associatedownergroup}" >
        <pnp:Members>
          <pnp:User Name="kevinc@bertonline.onmicrosoft.com"/>
          <pnp:User Name="c:0-.f|rolemanager|s-1-5-21-992416943-3518085199-3295385949-83050576"/>
        </pnp:Members>        
      </pnp:SiteGroup>
    </pnp:SiteGroups>

    <pnp:Permissions>  
      <pnp:RoleAssignments>
        <pnp:RoleAssignment Principal="ContosoMandatoryPermissions" RoleDefinition="Read"/>
      </pnp:RoleAssignments>
    </pnp:Permissions>
  </pnp:Security>
</pnp:ProvisioningTemplate>
```

#### Grant permissions directly to a user/group
Finally, you can also directly grant permissions to a user or group, so without nesting them inside a SharePoint group. Similar to the SharePoint groups you grant the added user/group a role like Read, Contribute or Full Control

```XML
<pnp:ProvisioningTemplate ID="security_add" Version="1" xmlns:pnp="http://schemas.dev.office.com/PnP/2017/05/ProvisioningSchema">
  <pnp:Security>
    <!-- Shows how to directly grant users and groups permissions to the site -->
    <pnp:Permissions>
      <pnp:RoleAssignments>
        <pnp:RoleAssignment Principal="kevinc@bertonline.onmicrosoft.com" RoleDefinition="Full Control" />
        <pnp:RoleAssignment Principal="c:0-.f|rolemanager|s-1-5-21-992416943-3518085199-3295385949-83050576" RoleDefinition="Read" />
      </pnp:RoleAssignments>
    </pnp:Permissions>
  </pnp:Security>
</pnp:ProvisioningTemplate>
```


### Configuring the application configuration file
Once you’ve configured a permissions PnP Provisioning template file it’s time to apply that to one or more site collections. Before you can run the application you however need to configure the application settings as described in below table:

Setting | Value
---------|----------
AppId | The ID of the defined Azure AD application e.g. e5808e8b-6119-44a9-b9d8-9003db04a882 
AzureTenant | The name of the Azure AD tenant you’re using e.g. bertonline.onmicrosoft.com
PfxCertificate | The certificate you used to setup app-only for the Azure AD application. This needs to be a PFX file which includes the certificate with its private key. E.g. C:\demo\BertOnlineAzureADAppOnlyDefaultPassword.pfx
PfxCertificatePassword | The password of the PFX certificate file
TenantAdmin | The url to your tenant admin center e.g. https://bertonline-admin.sharepoint.com 
ExcludeOD4BSites | Do you want to also grant the permissions to your OneDrive for Business sites? Default is false and OD4B sites are included.
NumberOfThreads | How many parallel threads do you want to spin up. Default value is 5 but you might bump this up to higher values to help improve performance.
SiteFilters | Which filter do you want to apply to identify the sites to operate on. This is an important parameter which is described in more detail in the next chapter.

> Note:
> See the [Granting access via Azure AD App-Only](https://docs.microsoft.com/en-us/sharepoint/dev/solution-guidance/security-apponly-azuread) article to learn how to configure an Azure AD application that you can use to run this solution.

#### Configuring the SiteFilters application setting
Using wildcard URL’s, you can control which site collections are processed by this application. This model allows you to define a very granular list by specifying the individual site collections but typically you would want to include a lot of site collections. If you want to tool to run for all site collections, you need to provide 2 filters: one for the OD4B site collections and one for the “other” site collections. 

*All sites in my tenant:*
https://bertonline.sharepoint.com/sites/*,https://bertonline-my.sharepoint.com/personal/*

*My test sites:*
https://bertonline.sharepoint.com/sites/test*

*One individual test site collection*
https://bertonline.sharepoint.com/sites/testwebapppolicy

> **IMPORTANT:**
> - Wild card URL’s are separated by a colon (,)
> - You need to specify at least one wild card URL per unique host name (like bertonline.sharepoint.com and bertonline-my.sharepoint.com) if you want to include site collections from these host names

## Running the application
### I’m testing
When you’re testing the application you can use a narrow scoped site filter as described in previous chapter, but you can also comment out the site filters. If that’s done the inside the code you’ll be able define your site filters + control other options like making it a single threaded run which is easier to debug.

```C#
if (!string.IsNullOrEmpty(siteFilters))
{
    string[] filters = siteFilters.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
    foreach(var filter in filters)
    {
        job.AddSite(filter);
    }
}
else
{
    // I'm debugging...
    job.UseThreading = false;
    job.AddSite("https://bertonline.sharepoint.com/sites/bert2");
}
```

### I’m having a Multi-Geo tenant
If your tenant is a Multi-Geo tenant then you need to remember to configure and schedule this application for each geo location in your Multi-Geo tenant.

<img src="https://telemetry.sharepointpnp.com/pnp/solutions/Governance.EnsurePolicy" />