# Mail Addin For Outlook Using Office 365 APIs (ADAL.JS, ANGULARJS, WEBAPI, AZURE AD) #

### Summary ###
This sample demonstrates mail add-in for Outlook which extracts data from a mail message and retrieves data from Office 365 APIs. Node.js is used for server side code, AngularJS for front-end and for authentication purposes Adal.js + Azure AD. Additionaly(not required to run) there is .NET WebApi project to show how we can retrive data from custom REST APIs. (projectUrl)

### Applies to ###
-  Outlook 2013

### Prerequisites ###
-  Office 365 Developer Subscription. See [Sign up for an Office 365 Developer Subscription and set up your tools and environment](https://msdn.microsoft.com/EN-US/library/office/fp179924.aspx)
-  Must have an Office 365 developer site. See [How to: Create a Developer Site within your existing Office 365 subscription](https://msdn.microsoft.com/en-us/library/office/jj692554.aspx)
- Azure accout is required - you will need to configure Azure Active Directory for this sample. See [Step 2: Register the sample with your Azure Active Directory tenant](https://github.com/AzureADSamples/SinglePageApp-AngularJS-DotNet#step-2--register-the-sample-with-your-azure-active-directory-tenant)
- Optionaly: you will run .NET WebApi project to show additional sample data (seen on picture as Reports, Employees) as an example how we can consume external REST APIs. Use this [agile9.outlook.context.db - Code First Entity Framework 6.0 Sample Project With Data](https://github.com/matejv1/agile9.outlook.context.db)

### Solution ###
Solution | Author(s)
---------|----------
Context - MAIL ADDIN FOR OUTLOOK | Matej Vodopivc (**Agile9.net**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | Aug 29rd 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Mail Addin For Outlook Using Office 365 APIs #
This code sample demonstrates the use of an add-in for Outlook for showing additional data to user from Office 365 APIs and custom REST APIs.

![](http://)

## 1. Building this sample ##
This sample consists of 3 primary components:

1. Node.js server side code - running localy (can be hosted anywhere, Azure for example fully supports Node.js arhitecture)
2. Addin for Office Manifest - defines how our add-in is activated within Outlook
3. Front-end code - HTML markup and AngularJS Javascript for interacting with the server-side API


### 1.1 Configure Azure Active Directory ###

This is already well documented. See [Step 2: Register the sample with your Azure Active Directory tenant](https://github.com/AzureADSamples/SinglePageApp-AngularJS-DotNet#step-2--register-the-sample-with-your-azure-active-directory-tenant)

Note: Make sure you 
	1. create Key 
	2. make changes to Manifest file (Download, oauth2AllowImplicitFlow: true, Upload back)
	3. give an app required permissions (Exchange, Sharepoint Online)

### 1.2 Configure App Settings ###

1. Open project location in Explorer. Open src/app.routes.js
2. Change tenant name (replace "agile9" with your Office 365 tenant name)

### 1.3 Install Node.js Dependecies ###

1. Open source location on local computer using Explorer. 
2. Open Command Prompt in this folder (Hold SHIFT + Right click -> Open Command Windows here)
3. Run: npm install

### 1.4 Upload Office Manifest to Exchange ###

1. Navigate to portal.office.com
2. Select Admin
3. In the left side menu select ADMIN -> Exchange
4. On Exchange admin center page select "add-ins" under "organization" group
5. Select Add from File and upload manifest

### 1.5 Running the sample

1. Follow steps in 1.2 to navigate to right location using CMD
2. Run: npm start
3. Open in Chrome browser: https://localhost:8443/#/ 
4. Accept SSL warning (certificate is not verifed by know authority)
5. Open https://outlook.office365.com and see the result
