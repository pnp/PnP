# Sample using Microsoft Graph with Apache Cordova and ADAL Cordova Plugin #

### Overview ###
This sample demonstrates how to use the Microsoft Graph API to retrieve data from 
Office 365 using the REST API and OData. The sample is intentionally simple and 
does not use any SPA frameworks,
data-binding library, jQuery, etc. It is not intended as a demonstration of a 
full-featured mobile app. You can target various Windows platforms as well as 
Android and iOS using the same JavaScript code.

The access token is obtained using the ADAL Cordova plugin. This is one of the 
core plugins in Visual Studio and is available from the config.xml editor.
This is an alternative to the Add Connected Service wizard that generates
a number of JavaScript files including a library (o365auth.js) that can be 
used to obtain tokens using an in-app browser to handle the user redirect 
to the authorization endpoint. Instead, the ADAL Cordova plugin uses the native
ADAL libraries for each platform and so is able to take advantage of native 
features such as token cacheing and hardened browsers.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Microsoft Graph

### Prerequisites ###
- Visual Studio Tools for Apache Cordova (VS-TACO setup option)
- ADAL Cordova plugin (cordova-plugin-ms-adal)

### Solution ###
Solution | Author(s)
---------|----------
Mobile.MicrosoftGraphCordova | Bill Ayers (@SPDoctor, spdoctor.com, flosim.com)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | March 15th 2016 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

![Running on Windows 10](MicrosoftGraphCordova.png)


Note: The current recommended pattern is to call acquireTokenSilentAsync first.
If a token can't be obtained silently (i.e. from the cache or by using a
refresh token), the "fail" callback invokes acquireTokenAsync which has its
prompt behaviour set to "always".

The intention is that acquireTokenAsync will always prompt in ADAL libraries moving forward. 

```javascript

    context.acquireTokenSilentAsync(resourceUrl, appId).then(success, function () {
      context.acquireTokenAsync(resourceUrl, appId, redirectUrl).then(success, fail);
    });

```