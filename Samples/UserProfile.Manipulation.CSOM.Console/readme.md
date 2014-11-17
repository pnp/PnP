# User profile manipulation using CSOM #

### Summary ###
This sample shows how a Tenant Administrator can manipulate user profile properties of the users in a Tenant using Client Side Object Model (CSOM).
It is using the latest SharePoint Online CSOM, which is exposing APIs also to update user profile properties.
You can download the latest version of the SharePoint online client SDK from following link - http://aka.ms/spocsom


### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
Capability will have to be enabled in the used tenant. This will happen gradually for all public tenants.

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