# User profile manipulation using CSOM #

### Summary ###
This sample shows how a Tenant Administrator can manipulate user profile properties of the users in a Tenant using Client Side Object Model (CSOM).
It is using the latest SharePoint Online CSOM, which is exposing APIs also to update user profile properties.
You can download the latest version of the SharePoint online client SDK from following link - http://aka.ms/spocsom


### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
This capability was introduced in the CSOM package released on September 2014. All Office 365 tenants should have this now enabled.

### Solution ###
Solution | Author(s)
---------|----------
UserProfile.Manipulation.CSOM.Console | Vardhaman Deshpande

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | November 14th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------
# Introduction #
Using CSOM for updating user profile properties was introduced in November 2014. Previously this was only possible with web services. This is scenario which is often needed for personalization purposes and to synchronize for example user profile properties which ware not sync'ed from the Azure AD to SharePoint user profile.

Here's typical process of synchronizing additional attributes from on-premises to the Office 365.

![Process picture with 4 steps](http://i.imgur.com/Jt4miQJ.png)

1. You can control what attributes are synchronized from the local AD to the Azure AD
2. Standardized set of attributes are synchronized from Azure AD to the SharePoint User Profile. You cannot modify this mapping in Office 365
3. To synchronize additional attributes or information, you can have custom tool which is accessing the AD or any other LOB system and synchronizes the needed attributes directly to the SharePoint user profile
4. SharePoint User Profile properties are available for different purposes in the SharePoint UI

Check more detailed introduction to the code from following blog post

- [Set another user's profile properties with CSOM](http://www.vrdmn.com/2014/11/set-another-users-profile-properties.html)


# Required Details #
You will need the Tenant Administrator details and the AccountName of the User whose properties you want to modify. Replace the following values with values in your Tenant.

```C#

//Tenant Admin Details
string tenantAdministrationUrl = "https://yourtenant-admin.sharepoint.com/";
string tenantAdminLoginName = "admin@yourtenant.onmicrosoft.com";
string tenantAdminPassword = "Password";

//AccountName of the user whos property you want to update.
string UserAccountName = "i:0#.f|membership|anotheruser@yourtenant.onmicrosoft.com";
```


# Modify Single Value User Profile Property #
Use the following code to modify a single value user profile property of a user

```C#
clientContext.Credentials = new SharePointOnlineCredentials(tenantAdminLoginName, passWord);

// Get the people manager instance for tenant context
PeopleManager peopleManager = new PeopleManager(clientContext);

// Update the AboutMe property for the user using account name.
peopleManager.SetSingleValueProfileProperty(UserAccountName, "AboutMe", "Value updated from CSOM");

clientContext.ExecuteQuery();
```

# Modify Multi Value User Profile Property #
Use the following code to modify a multi value user profile property of a user

```C#

clientContext.Credentials = new SharePointOnlineCredentials(tenantAdminLoginName, passWord);

// List Multiple values
List<string> skills = new List<string>() { "SharePoint", "Office 365", "C#", "JavaScript" };

// Get the people manager instance for tenant context
PeopleManager peopleManager = new PeopleManager(clientContext);

// Update the SPS-Skills property for the user using account name from profile.
peopleManager.SetMultiValuedProfileProperty(UserAccountName, "SPS-Skills", skills);

clientContext.ExecuteQuery();

```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/UserProfile.Manipulation.CSOM.Console" />