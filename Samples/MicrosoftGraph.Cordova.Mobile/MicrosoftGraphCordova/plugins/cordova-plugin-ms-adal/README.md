# Active Directory Authentication Library (ADAL) plugin for Apache Cordova apps

Active Directory Authentication Library ([ADAL](https://msdn.microsoft.com/en-us/library/azure/jj573266.aspx)) plugin provides easy to use authentication functionality for your Apache Cordova apps by taking advantage of Windows Server Active Directory and Windows Azure Active Directory.
Here you can find the source code for the library.

  * [ADAL for Android](https://github.com/AzureAD/azure-activedirectory-library-for-android),
  * [ADAL for iOS](https://github.com/AzureAD/azure-activedirectory-library-for-objc),
  * [ADAL for .NET](https://github.com/AzureAD/azure-activedirectory-library-for-dotnet).

This plugin uses native SDKs for ADAL for each supported platform and provides single API across all platforms. Here is a quick usage sample:

```javascript
var AuthenticationContext = Microsoft.ADAL.AuthenticationContext;

AuthenticationContext.createAsync(authority)
.then(function (authContext) {
    authContext.acquireTokenAsync(resourceUrl, appId, redirectUrl)
    .then(function (authResponse) {
        console.log("Token acquired: " + authResponse.accessToken);
        console.log("Token will expire on: " + authResponse.expiresOn);
    }, fail);
}, fail);
```

__Note__: You can use `AuthenticationContext` synchronous constructor as well:

```javascript
authContext = new AuthenticationContext(authority);
authContext.acquireTokenAsync(resourceUrl, appId, redirectUrl).then(function (authRes) {
    console.log(authRes.accessToken);
    ...
});
```

For more API documentation see [sample application](https://github.com/AzureAD/azure-activedirectory-library-for-cordova/tree/master/sample) and JSDoc for exposed functionality stored in [www](https://github.com/AzureAD/azure-activedirectory-library-for-cordova/tree/master/www) subfolder.

## Supported platforms

  * Android
  * iOS
  * Windows (Windows 8.0, Windows 8.1 and Windows Phone 8.1)

## Known issues and workarounds

## 'Class not registered' error on Windows

If you are using Visual Studio 2013 and see 'WinRTError: Class not registered' runtime error on Windows make sure Visual Studio [Update 5](https://www.visualstudio.com/news/vs2013-update5-vs) is installed.

## Multiple login windows issue

Multiple login dialog windows will be shown if `acquireTokenAsync` is called multiple times and the token could not be acquired silently (at the first run for example). Use a [promise queueing](https://www.npmjs.com/package/promise-queue)/semaphore logic in the app code to avoid this issue.

## Installation Instructions

### Prerequisites

* [NodeJS and NPM](https://nodejs.org/)

* [Cordova CLI](https://cordova.apache.org/)

  Cordova CLI can be easily installed via NPM package manager: `npm install -g cordova`

* Additional prerequisites for each target platform can be found at [Cordova platforms documentation](http://cordova.apache.org/docs/en/edge/guide_platforms_index.md.html#Platform%20Guides) page:
 * [Instructions for Android](http://cordova.apache.org/docs/en/edge/guide_platforms_android_index.md.html#Android%20Platform%20Guide)
 * [Instructions for iOS](http://cordova.apache.org/docs/en/edge/guide_platforms_ios_index.md.html#iOS%20Platform%20Guide)
 * [Instructions for Windows] (http://cordova.apache.org/docs/en/edge/guide_platforms_win8_index.md.html#Windows%20Platform%20Guide)

### To build and run sample application

  * Clone plugin repository into a directory of your choice

    `git clone https://github.com/AzureAD/azure-activedirectory-library-for-cordova.git`

  * Create a project and add the platforms you want to support

    `cordova create ADALSample --copy-from="azure-activedirectory-library-for-cordova/sample"`

    `cd ADALSample`

    `cordova platform add android`

    `cordova platform add ios`

    `cordova platform add windows`

  * Add the plugin to your project

    `cordova plugin add ../azure-activedirectory-library-for-cordova`

  * Build and run application: `cordova run`.


## Setting up an Application in Azure AD

You can find detailed instructions how to set up a new application in Azure AD [here](https://github.com/AzureADSamples/NativeClient-MultiTarget-DotNet#step-4--register-the-sample-with-your-azure-active-directory-tenant).

## Tests

This plugin contains test suite, based on [Cordova test-framework plugin](https://github.com/apache/cordova-plugin-test-framework). The test suite is placed under `tests` folder at the root or repo and represents a separate plugin.

To run the tests you need to create a new application as described in [Installation Instructions section](#installation-instructions) and then do the following steps:

  * Add test suite to application

    `cordova plugin add ../azure-activedirectory-library-for-cordova/tests`

  * Update application's config.xml file: change `<content src="index.html" />` to `<content src="cdvtests/index.html" />`
  * Change AD-specific settings for test application at the beginning of `plugins\cordova-plugin-ms-adal\www\tests.js` file. Update `AUTHORITY_URL`, `RESOURCE_URL`, `REDIRECT_URL`, `APP_ID` to values, provided by your Azure AD. For instructions how to setup an Azure AD application see [Setting up an Application in Azure AD section](#setting-up-an-application-in-azure-ad).
  * Build and run application.

## Windows Quirks ##
[There is currently a Cordova issue](https://issues.apache.org/jira/browse/CB-8615), which entails the need of the hook-based workaround.
The workaround is to be discarded after a fix is applied.

### Using ADFS/SSO
To use ADFS/SSO on Windows platform (Windows Phone 8.1 is not supported for now) add the following preference into `config.xml`:
`<preference name="adal-use-corporate-network" value="true" />`

`adal-use-corporate-network` is `false` by default.

It will add all needed application capabilities and toggle authContext to support ADFS. You can change its value to `false` and back later, or remove it from `config.xml` - call `cordova prepare` after it to apply the changes.

__Note__: You should not normally use `adal-use-corporate-network` as it adds capabilities, which prevents an app from being published in the Windows Store.

## Copyrights ##
Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License"); you may not use these files except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
