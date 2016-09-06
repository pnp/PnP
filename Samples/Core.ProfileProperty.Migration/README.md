# PROFILE PROPERTY MIGRATION #

### Summary ###
A set of migration console applications used to export single and multi-valued user profile properties from an on-premises deployment and write those properties to an intermediate store. Secondly, we then update SharePoint Online Tenant via that intermediate store.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises
-  SharePoint 2010 


### Solution ###
Solution | Author(s)
---------|----------
Core.ProfileProperty.Migration | Mark Franco (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | April 3, 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# General #
Updating and reading of user profile properties happens via the user profile web service (userprofileservice.asmx). If you’re using Office 365 Multi-Tenant you can launch this service from the my-site host (e.g. https://contoso-my.sharepoint.com/_vti_bin/userprofileservice.asmx) or from tenant administration (e.g. https://contoso-admin.sharepoint.com/_vti_bin/userprofileservice.asmx). It’s important that if you want to update all user profile properties for all users that you use the user profile web service in combination with the tenant administration site. 
Typical use cases for this are:
- These tools are architected with a simple migration mechanism (XML serialization), making the process of bringing on premises profile data to SPO a two-step process for robustness.
	- Extracting user profile information from an on premises SP2010/SP2013 environment and persist the data to disk via XML serialization. In the case of this example “Ask me about”, and “About me” Properties are used only.
	- Updating user profile properties in SPO using the import tool’s output (XML File with user profile data).
	
**IMPORTANT**:
Use this capability with care as this allows you to override “system properties” such as workemail which contain data coming from Exchange Online in a typical Office 365 MT or D deployment.


# EXTRACT USER PROFILE PROPERTIES FROM SP2010/SP2013 #

This scenario shows how to read single and multi-value user profile properties. To make this work you need to run the code on a server on your on-premises farm with farm level permissions. This tool’s code base uses server side OM.
Note: the SharePoint Server does not require connectivity as we are pulling data and persisting to disk as part of a two-step process.

This scenario is handles by the Contoso.ProfileProperty.Migration.Extract, version 1.0 console application.

*Note*: you will have to change the following values in the app.config file to suit your environment before proceeding further:

![Setting options as picture](http://i.imgur.com/jvfY26z.png)


## INITIALIZATION ##
**Step 1**
We must add the reference ‘Microsoft.Office.Server.UserProfiles.dll’ so we have access to the UserProfileManager class and connect as follows:
```C#
SPServiceContext svcContext = SPServiceContext.GetContext(mySite);
UserProfileManager profileManager = new UserProfileManager(svcContext);
```
	
**Step 2**
Enumerate all user profiles as follows:

```C#
foreach (UserProfile spUser in profileManager)
{ … }
```


## READ A SIMPLE USER PROFILE PROPERTY ##
![UI of needed code](http://i.imgur.com/MfGhSsP.png)

## READ A COMPLEX USER PROFILE PROPERTY ##
![UI of used code](http://i.imgur.com/1sKb9fM.png)

## OUTPUT ##
Program Output:
![Console application output](http://i.imgur.com/CypFjV8.png)

Output file:
![Output xml from solution](http://i.imgur.com/5ZGRh7x.png)



# IMPORT USER PROFILE PROPERTIES TO SPO #

This scenario shows how to user profile properties via an XML file. To make this work you need to specify tenant administration credentials: if you specify regular user credentials you’ll not be able to update the user profile properties of other users.
This scenario is handled by the Contoso.ProfileProperty.Migration.Import, version 1.0 console application.
Note: you will have to change the following values in the app.config file to suit your environment before proceeding further:

![Configuration properties as a table](http://i.imgur.com/ZH3wzYK.png)

## INITIALIZATION ##

![Code for initialization](http://i.imgur.com/N2zGQG4.png)

## WRITE A SIMPLE USER PROFILE PROPERTY ##

![Code for updating user profile property](http://i.imgur.com/UOrKZVv.png)

## WRITE A COMPLEX USER PROFILE PROPERTY ##
![Code for updating complex user profile property](http://i.imgur.com/dxSQdJU.png)

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.ProfileProperty.Migration" />