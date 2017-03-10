# Office 365 Api - Groups Explorer#

### Summary ###
The companion web application lists all groups in the user's tenant, along with all the properties.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
This sample requires the Office 365 API version released on November 2014. See http://msdn.microsoft.com/en-us/office/office365/howto/platform-development-overview for more details.

### Solution ###
Solution | Author(s)
---------|----------
Office365Api.Groups | Paul Schaeflein (Schaeflein Consulting, @paulschaeflein)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | February 8th 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Exploring the Office 365 Groups API #
This sample is provided to aid in the review of properties and relationships of Office 365 Groups.
More information can be found in the blog post at http://www.schaeflein.net/exploring-the-office-365-groups-api/.



# The ASP.NET MVC Sample #
This section describes the ASP.NET MVC sample included in the current solution.

## Prepare the scenario for the ASP.NET MVC Sample ##
The ASP.NET MVC sample application will use the new Microsoft Graph API's to perform the following list of tasks:

-  Read list of groups in the current user's directory
-  Read the conversations, events and files in "unified" groups
-  List the groups to which the current user has joined

In order to run the web application you will need to register it in your development Azure AD tenant.
The web application uses OWIN and OpenId Connect to Authenticate against the Azure AD that sits under the cover of your Office 365 tenant.
You can find more details about OWIN and OpenId Connect here, as well as about registering you app on the Azure AD tenant: http://www.cloudidentity.com/blog/2014/07/28/protecting-an-mvc4-vs2012-project-with-openid-connect-and-azure-ad/ 

After registering the app in the Azure AD tenant, you will have to configure the following settings in the web.config file:

		<add key="ida:ClientId" value="[Your ClientID here]" />
		<add key="ida:ClientSecret" value="[Your ClientSecret here]" />
		<add key="ida:TenantId" value="[Your TenantId here]" />
		<add key="ida:Domain" value="your_domain.onmicrosoft.com" />

# Under the cover of the sample #
The application is coded against the beta endpoint of the graph api. The GroupsController class specifies the URL for each call:

```
string apiUrl = String.Format("{0}/beta/myorganization/groups/{1}/conversations/{2}/threads", 
                              SettingsHelper.MSGraphResourceId, 
                              id, itemId);
```

The user interface uses the Office UI Fabric (http://dev.office.com/fabric). There are a few custom DisplayTemplate views that handle the styling required of the fabric css.

## Credits ##
The multi-tenancy with ASP.NET MVC and OpenID Connect is provided thanks to the GitHub project available here:
https://github.com/Azure-Samples/active-directory-dotnet-webapp-multitenant-openidconnect

Credits to https://github.com/dstrockis and https://github.com/vibronet.

The Office Fabric UI styling was aided by a blog post here: http://chakkaradeep.com/index.php/using-office-ui-fabric-in-sharepoint-add-ins/

Credit to https://github.com/chakkaradeep

<img src="https://telemetry.sharepointpnp.com/pnp/samples/MicrosoftGraph.Office365.GroupsExplorer" />