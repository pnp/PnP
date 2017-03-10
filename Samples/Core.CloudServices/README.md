# Host SharePoint Apps on Azure Cloud Services #

### Summary ###
This sample shows how one can leverage the Azure Cloud Services (web and worker roles) to host SharePoint provider hosted apps. The code running on Azure shows how to make use of the tenant administration CSOM.

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
Azure subscription, recent Azure SDK installed

### Solution ###
Solution | Author(s)
---------|----------
Core.CloudServices | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
2.1  | August 4th 2015 | Updated to VS 2015 and with Azure SDK 2.7
2.0  | March 25th 2014 | Minor fixes, documentation updates
1.0  | November 6th 2013 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# HOW TO PREPARE YOUR VISUAL STUDIO 2015 ENVIRONMENT TO COMBINE AZURE AND SHAREPOINT APPS #
This sample is based on Visual Studio 2015 in combination with the Azure SDK version 2.7. It’s important to at least use these versions in order to get a first class experience in Visual Studio. We do want to have a solution that when pressing F5 spins up the Azure emulator, deploys the solution to your developer site collection and then runs your solution on the emulator. To realize this the following steps need to be followed (note that there are also alternative options, but below approach is used to build this sample).

*Should work fine with VS2013 as well with Azure SDK version 2.7*

## CREATE AN AZURE CLOUD SERVICES PROJECT ##
Open VS2013 and create a new Azure Cloud Services project with a web role and a worker role:

![Project template selection in Visual Studio](http://i.imgur.com/O1ggSsr.png)

![Template selection for Azure Cloud Service](http://i.imgur.com/eJseiWO.png)

![New ASP.NET MVC template dialog](http://i.imgur.com/C75QohE.png)

## CONVERT TO AZURE WEB ROLE TO A SHAREPOINT APP PROJECT ##
Right click the Azure web role project and use the convert menu to “transform” it into a SharePoint App project:

![Convert option from project context menu](http://i.imgur.com/se8K5lf.png)

After the conversion completed you should see a similar project structure in your solution. Notice the WebRole1.SharePoint project that has been added:

![Visual Studio Solution structure](http://i.imgur.com/1ZUIdyJ.png)

## ENSURE BOTH THE AZURE PROJECT AS THE SHAREPOINT PROJECT ARE STARTUP PROJECTS ##
To make the F5 experience work it’s important that the SharePoint App and the Azure project are starting on F5. This can be configured via the solution properties:

![The property page for the solution Windows Azure1. Under Common Properties, Startup Project is selected. The radio button Multiple startup projects is selected.](http://i.imgur.com/Jlj1KUo.png)

## IMPORTANT: ENSURE YOU’VE A SHAREPOINT PUBLISHING PROFILE BEFORE PUBLISHING YOUR SHAREPOINT APP ##
If you’ve created the solution as described above and you do want to “Publish” your SharePoint app then this seems to work, but it’s not working correctly since the publishing wizard will not substitute your client ID and remoteAppUrl token because it’s lacking a SharePoint publishing profile. 

![PUblish selection in context menu](http://i.imgur.com/gUJXDdk.png)

This is due to a bug in Visual Studio 2013, but luckily there’s an easy workaround. The very first time you do want to publish your app you should follow below steps, subsequent publishing actions will just work fine. First we “unload” the Azure project:

![Unload option in project context menu](http://i.imgur.com/vRdnOUX.png)

Once this is done publish your SharePoint app by creating a publishing profile and packaging your app. After publishing you should see a folder named PublishProfiles with a pubxml file in the web project of your SharePoint App:

![Created publishing profile file](http://i.imgur.com/Youz0XH.png)

Once this is done you can reload the Azure project and you’re fine for all future publish actions.

# HOW USE THE SHAREPOINT CSOM FROM AZURE
There are two ways to get a client context that can be used with the both the SharePoint CSOM as the tenant administration CSOM and depending on the method you choose the configuration of your Azure web and worker roles requires additional configuration. A more complex could for example be a site provisioning system that uses the tenant administration CSOM to create a site collection and then use the SharePoint CSOM to activate features, add web parts to the home page,..

**Option 1:** using the SharePointOnlineCredentials class

```C#
ClientContext cc = new AuthenticationManager().GetSharePointOnlineAuthenticatedContextTenant(messageParts[1],"TenantAdminUser", "TenantAdminPassword");
```


**Option 2:** using an OAuth app only token

```C#
ClientContext cc = new AuthenticationManager().GetAppOnlyAuthenticatedContext(messageParts[1],"Realm","AppId","AppSecret");
```

Once you’ve a client context you can use it to work with SharePoint:
```C#
//Update the site title
cc.Web.Title = messageParts[0];
cc.Web.Update();
cc.ExecuteQuery();
```

## CLIENTCONTEXT BASED ON THE SHAREPOINTONLINECREDENTIALS CLASS ##
In this model we’re going to work with a SharePoint tenant administrator account and password that is being used to construct an instance of the SharePointOnlineCredentials class. This class is then used to derive the correct ClientContext object. To make this model operate in an Azure web or worker role the below configuration steps are required.

### INSTALLATION OF ONLINE SERVICES SIGN-IN ASSISTANT AND SHAREPOINT CSOM LIBRARIES IS REQUIRED ###
To install these two components we’ve added the installation packages to the project (notice the build action and copy to output directory settings):

![MSI installer in project](http://i.imgur.com/UODVckE.png)

Next we’ve foreseen a startup section in the servicedefinition.csdef file in the Azure project:

```XML
<Startup>
  <Task commandLine="startup.cmd" executionContext="elevated" taskType="simple">
    <Environment>
      <Variable name="EMULATED">
        <RoleInstanceValue xpath="/RoleEnvironment/Deployment/@emulated" />
      </Variable>
    </Environment>
  </Task>
</Startup>
```

If we take a look at the startup.cmd we see (simplified) this:
```cmd
@ECHO off

if "%EMULATED%"=="true" goto :EOF
 
ECHO "Installing the SharePoint CSOM library" >> log.txt
msiexec.exe /I "sharepointclientcomponents_x64.msi" /qn
"Installing Microsoft Online Services Sign In Assistant" >> log.txt
msiexec.exe /I "msoidcli_64bit.msi" /qn
ECHO "Completed SharePoint CSOM + SIA Installation" >> log.txt
```

### ENSURE THAT THE AZURE SERVICE IS RUNNING UNDER SYSTEM ACCOUNT INSTEAD OF NETWORK ACCOUNT ###
The Online Services Sign-In assistant does not operate when being called from a process running under the Network service, so we need to run under the System account. To change this add the Runtime element in the servicedefinition.csdef:

```XML
<WorkerRole name="Core.CloudServices.Worker" vmsize="Small">
  <Runtime executionContext="elevated" />
```

This will make an Azure worker role launch its host process under the system account and everything works fine, however for a web role this setting does not affect how the IIS application pools are configured which will host your web project. To fix this you need to change the application pool accounts to run under the system accounts via custom code (**SetAppPoolIdentity method**) that’s executed during role start:

```C#
// Only change application pool account when running in Azure, no need to change 
// this for the emulator as the emulator requires you to run with 
// administrative privileges
if (!RoleEnvironment.IsEmulated)
{
  // Use the SetAppPoolIdentity method in case you want to use the tenant
  // administration CSOM library in combination with specifying credentials 
  // via the SharePointOnlineCredentials class
  SetAppPoolIdentity();
}
```

## CLIENTCONTEXT BASED ON AN OAUTH TOKEN WITH TENANT LEVEL PERMISSIONS ##
Using a clientcontext based on an OAuth token actually is easier: there are no additional Azure configurations required! The only thing you need to do is register your app, grant it tenant level permissions and use the TokenHelper.GetAppOnlyAccessToken and TokenHelper.GetClientContextWithAccessToken methods to construct a clientcontext based on an app only OAuth token.

```C#
// Use (Get-MsolCompanyInformation).ObjectID to obtain Target/Tenant realm: <guid>
//
// Manually register an app via the appregnew.aspx page and generate an App ID and 
// App Secret. The App title and App domain can be a simple string like "MyApp"
//
// Update the AppID in your worker role settings
//
// Add the AppSecret in your worker role settings 
//
// Manually set the permission XML for you app via the appinv.aspx page:
// 1/ Lookup your app via its AppID
// 2/ Paste the permission XML and click on create
//
// Sample permission XML:
// <AppPermissionRequests AllowAppOnlyPolicy="true">
//   <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl" />
// </AppPermissionRequests>
//
// As you're granting tenant wide full control to an app the appsecret is as important
// as the password from your SharePoint administration account!
```

# HOW TO MAKE THE APP ID AND APP SECRET CONFIGURABLE FROM THE AZURE MANAGEMENT PORTAL #
This example shows how to make the relevant SharePoint app parameters (ID, secret, Realm) configurable from within the Azure management portal. To realize this a slightly adapted version of TokenHelper.cs is used in the OfficeDevPnP Core project in which the relevant properties can be read as properties. OfficeDevPnP Core provides us with an AuthenticationManager class that’s used to obtain the client context objects for both the sample using the SharePointOnlineCredentials class and the OAuth App Only token sample.

```C#
ClientContext cc = new AuthenticationManager().GetSharePointOnlineAuthenticatedContextTenant(messageParts[1],"TenantAdminUser", "TenantAdminPassword");

ClientContext cc = new AuthenticationManager().GetAppOnlyAuthenticatedContext(messageParts[1],"Realm","AppId","AppSecret");
```

# HOW TO ENCRYPT CONFIGURATION DATA IN AZURE #
In the above samples you really would want to secure your password or clientsecret since these both contain tenant level permissions. Since we’re running on Azure and can have multiple instances of web and worker roles we need an encryption mechanism that is machine independent and therefore we’ve opted use certificate based encryption. This means that data is encrypted based on the public key of the certificate, but in order to decrypt the data one requires the private key of the certificate. 

## SETUP THE CERTIFICATE IN AZURE AND IN YOUR SOLUTION ##
First you need to have a certificate in PFX format which contains a private key. You’ll need to go to your cloud service in the Azure management portal (https://manage.windowsazure.com), click on certificates and upload your certificate:

![Azure Portal UI](http://i.imgur.com/aF436Ym.png)

![Upload certificate UI](http://i.imgur.com/G89KL8x.png)

Next to that you should also install the certificate on your local machine. Once that’s done you can link your azure web and worker projects to the certificate by updating the relevant settings (notice the SSL entry):

![Certificates option in VS project properties](http://i.imgur.com/7tGhPf2.png)

## ENCRYPT/DECRYPT DATA USING THE CERTIFICATE ##
To encrypt data the sample contains a very simple winforms application (Core.CloudServices.Encryptor) that allows you to specify the thumbprint of the certificate to use, the value to encrypt and then shows the resulting encrypted text:

![Custom UI for certificate encrypt or decrypt](http://i.imgur.com/V9RyYKr.png)

### Note: ###
To decrypt the data the process doing the decryption needs to have access to the private key of the certificate. Inside a deployed Azure role this is taken care of by Azure, but in your development environment this means that you **do need to Visual Studio as administrator.**

Encryption/decryption is handled via the EncryptionUtiltity class in the from the OfficeDevPnP Core project.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.CloudServices" />