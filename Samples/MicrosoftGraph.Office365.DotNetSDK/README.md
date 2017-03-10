# Samples about the Microsoft Graph API SDK for .NET

### Summary ###
This is a sample solution that illustrates how to use the Microsoft Graph API SDK for .NET.
The solution includes:
* A console application, which uses the new MSAL (Microsoft Authentication Library) preview to 
authenticate against the new v2 Authentication endpoint
* An ASP.NET MVC web application, which uses the ADAL (Azure Active Directory Authentication Library) to
authenticate against the Azure AD endpoint

This sample is part of the code samples related to the book ["Programming Microsoft Office 365"](https://www.microsoftpressstore.com/store/programming-microsoft-office-365-includes-current-book-9781509300914) written by [Paolo Pialorsi](https://twitter.com/PaoloPia) and published by Microsoft Press.

### Applies to ###
-  Microsoft Office 365

### Solution ###
Solution | Author(s) | Twitter
---------|-----------|--------
MicrosoftGraph.Office365.DotNetSDK.sln | Paolo Pialorsi (PiaSys.com) | [@PaoloPia](https://twitter.com/PaoloPia)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | May 12th 2016 | Initial release

### Setup Instructions ###
In order to play with this sample, you need to:

-  Sign up for a developer subscription for Office 365 [Office Dev Center](http://dev.office.com/), if you don't have one
-  Register the web application in [Azure AD](https://manage.windowsazure.com/) in order to get a ClientID and a Client Secret 
-  Configure the Azure AD application with the following delegated permissions for Microsoft Graph: View users' basic profile, View users' email address
-  Update the web.config file of the web application with proper settings (ClientID, ClientSecret,Domain,TenantID)
-  Register the console application for the v2 Authentication endpoint in the new [Application Registration Portal](https://apps.dev.microsoft.com/) 
-  Configure the .config file of the console application with proper settings (MSAL_ClientID)

 
<img src="https://telemetry.sharepointpnp.com/pnp/samples/MicrosoftGraph.Office365.DotNetSDK" />