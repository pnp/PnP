# Office 365 API demo applications #

### Summary ###
This Solution show the output of various Office 365 API calls in a console alike output format, leveraging a WPF application, as well as in an ASP.NET MVC web application. The goal of this sample is to see the new API while keeping focus on the API calls themselves and less on the UI layer they're hosted in.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
This sample requires the Office 365 API version released on November 2014. See http://msdn.microsoft.com/en-us/office/office365/howto/platform-development-overview for more details.

### Solution ###
Solution | Author(s)
---------|----------
Office365Api.Overview | Bert Jansen (**Microsoft**), Paolo Pialorsi (**PiaSys.com**, @PaoloPia)

### Version history ###
Version  | Date | Comments
---------| -----| --------
4.0  | January 22nd 2015 | Updated to ADAL 2.14, refactored, added ASP.NET MVC sample with multi-tenancy (Paolo Pialorsi)
3.0  | January 7th 2015 | Updated to Office 365 API RTM and ADAL 2.13 (Paolo Pialorsi)
2.0  | August 12th 2014 | Switched to WPF add-in and added documentation (Bert Jansen)
1.0  | July 29th 2014 | Initial release (Bert Jansen)

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# The WPF Sample #
This section describes the Windows Desktop WPF sample included in the current solution.

# Prepare the scenario for the WPF Sample #
The WPF application will use the new Office 365 API's to perform the following list of tasks:
-  Discover the current user's OneDrive URL
-  Discover the current user's Mail URL
-  List the files and folders from the user's OneDrive
-  Upload a file to the "Shared with everyone" folder in the user's OneDrive
-  List all files and folders in the "Shared with everyone" folder of the user's OneDrive
-  Retrieve top 50 mails from the current user's "Inbox", just print the first 10
-  Send an email as the current user with the sent mail ending up in the user's "Sent items" mailbox folder
-  Create an email in the current user's "Drafts" mailbox folder
-  Get all users from Azure AD, just print the first 10

For these tasks to succeed you need to provide some input before you run the application. This is done by changing the text fields in the UI, just after launching the WPF project + by registering an application in Azure AD. You can do this by right-clicking the **Office365Api.Demo** project -> **Add** -> **Connected Service**. Select **Office 365 APIs** and click on **Register your App**:

![Connected Service UI](http://i.imgur.com/vksc2KD.png)

Click **Yes** to register an Azure AD App and then give it the permissions as shown below:

![Permission list](http://i.imgur.com/uhQpqHt.png)


## Run the WPF sample ##
When you run the sample you'll see some text fields, a window with a big button named "Run demo" and a black output section. Fill out the text fields with proper values, select a file to upload by browsing the file system, and click on the "Run demo" button to trigger the demo. What will first happen is that you need to logon with an Office 365 user account.

![Signing in to Azure AD](http://i.imgur.com/852IH4o.png)


Once you've logged on the Office 365 API will ask you for permissions: you need to consent that the app access your data for the listed categories:

![Consent permissions](http://i.imgur.com/M9D343S.png)


After those 2 steps are done the app can run and use all the API's to do it's work. The output is shown in console style:

![App running in console](http://i.imgur.com/vLcdlrL.png)

----------

# The ASP.NET MVC Sample #
This section describes the ASP.NET MVC sample included in the current solution.

## Prepare the scenario for the ASP.NET MVC Sample ##
The ASP.NET MVC sample application will use the new Office 365 API's to perform the following list of tasks:
-  Discover the current user's OneDrive URL
-  Discover the current user's Mail URL
-  List the files from the current user's OneDrive
-  Retrieve contacts from the current user's Address Books, just print the first 10
-  Retrieve top 50 mails from the current user's "Inbox", just print the first 10
-  Send a mail as the current user with the sent mail ending up in the user's "Sent items" mailbox folder

In order to run the web application you will need to register it in your development Azure AD tenant.
The web application uses OWIN and OpenId Connect to Authenticate against the Azure AD that sits under the cover of your Office 365 tenant.
You can find more details about OWIN and OpenId Connect here, as well as about registering you app on the Azure AD tenant: http://www.cloudidentity.com/blog/2014/07/28/protecting-an-mvc4-vs2012-project-with-openid-connect-and-azure-ad/ 

After registering the app in the Azure AD tenant, you will have to configure the following settings in the web.config file:

    <add key="ida:FederationMetadataLocation" value="https://login.windows.net/[your-tenant].onmicrosoft.com/FederationMetadata/2007-06/FederationMetadata.xml" />
    <add key="ida:Realm" value="https://[your-tenant-domain]/Office365Api.MVCDemo" />
    <add key="ida:AudienceUri" value="https://[your-tenant-domain]/Office365Api.MVCDemo" />
    <add key="ida:ClientId" value="[Your App ClientId]" />
    <add key="ida:Password" value="[Your App Shared Secret]" />

## Run the ASP.NET MVC sample ##
To run the ASP.NET MVC sample you can you use the development Azure AD tenant where you registered the app before, or you can sign up a new tenant.
To Sign Up a new Office 365 tenant, or at least a single Office 365 user of another tenant, for the sample application, start the Web Application (Office365Api.MVCDemo) and choose the "Sign Up" menu item.
You will be prompted with the following screen:

![The Sign Up Tenant page](http://i.imgur.com/KbakAEe.png)

Provide the name of your tenant (something like <tenant>.onmicrosoft.com) and, if you are a Tenant Global Administrator and you want to make the web application available to all the users of your tenant, check the "Check this if you are an administrator and you want to enable the app for all your users" option.
You will have to login through Azure AD using an authorized account for the tenant you selected.
After that, you will have to "accept" the disclaimer that will show up, informing you about the permissions that the web application requires against your account and/or tenant. Click "Accept" to proceed.

![App UI](http://i.imgur.com/O7YzigB.png)

You will be redirected back to the web application. And you will have the confirmation that you properly signed up.

![Consent permissions](http://i.imgur.com/j6OX3KU.png)

Now you can click the "Office 365 API" menu item. You will see the following page:

![Office 365 API menu option](http://i.imgur.com/24DwisF.png)

Press each of the buttons, in order to play with the capabilites demonstrated by the web application.
Here you can see the sample result of pressing the "Send Mail" button.

![UI options to execute](http://i.imgur.com/9BpMZZG.png)


----------

# Under the cover of the samples #

## Some explanation about the API's themselves ##
The sample applications leverage the following client libraries, which are available as NuGet Packages:
- Microsoft.Azure.ActiveDirectory.GraphClient
- Microsoft.IdentityModel.Clients.ActiveDirectory
- Microsoft.Office365.Discovery
- Microsoft.Office365.OAuth.Windows
- Microsoft.Office365.OutlookServices
- Microsoft.Office365.SharePoint

The overall job is done in the helper library project (Office365Api.Helpers).
There you can find a bunch of classes which leverage the native client libraries for the Office 365 API, providing an high level approach for consuming the services.
Moreover, the helper classes can be used both on desktop and web application. The inner code of the helper classes will handle the two kind of applications, based on how you invoke the EnsureAuthenticationContext method of the AuthenticationHelper class.
While used inside a desktop application, the helper classes will behave as the currently logged in Office 365 user.
While used inside a web application, the helper classes will behave as the user impersonated in the current thread handling the current ASP.NET MVC request and logged in Office 365.

## Credits ##
The multi-tenancy with ASP.NET MVC and OpenID Connect is provided thanks to the GitHub project available here:
https://github.com/AzureADSamples/WebApp-WebAPI-MultiTenant-OpenIdConnect-DotNet

Credits to https://github.com/dstrockis and https://github.com/vibronet.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Office365Api.Overview" />