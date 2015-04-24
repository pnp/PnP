# UPS Package Tracker for Outlook 2013 #

### Summary ###
This sample demonstrates how to create an App for Outlook which extracts data from a mail message, retrieves data from an external service and formats the data for display.

### Applies to ###
-  Outlook 2013

### Prerequisites ###
-  Office 365 Developer Subscription. See [Sign up for an Office 365 Developer Subscription and set up your tools and environment](https://msdn.microsoft.com/EN-US/library/office/fp179924.aspx)
-  Must have an Office 365 developer site. See [How to: Create a Developer Site within your existing Office 365 subscription](https://msdn.microsoft.com/en-us/library/office/jj692554.aspx)
-  In order to run the sample you must create a developer account on the [UPS Developer Kit](https://www.ups.com/upsdeveloperkit) site. The username, password and access license number from UPS must be placed into the web.config file (see below).

### Solution ###
Solution | Author(s)
---------|----------
UPSPackageTracker.sln | Doug Perkes

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | Apr 23rd 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Tracking UPS Package Delivery in Outlook #
This code sample demonstrates the use of an App for Outlook for tracking package shipments from UPS.

![](http://i.imgur.com/BXXM3sF.png)

## 1. Building the Solution ##
The UPS Tracking App for Outlook consists of 4 primary components:

1. The UPS Tracking Service - provided by UPS for retrieving tracking data.
2. Server-side API - custom JSON API which wraps the UPS service and caches the result in Azure Table Storage.
3. App for Office Manifest - defines how our app is activated within Outlook
4. Front-end code - HTML markup and Javascript for interacting with the server-side API

### 1.1. The UPS Tracking Service ###
The UPS tracking service hosted by UPS provides an XML request & response schema for accessing package tracking data. To use the service you must create an account on the [https://www.ups.com/upsdeveloperkit](https://www.ups.com/upsdeveloperkit "UPS Developer Kit") site after which you will be given a license number. You must use the username, password and license number to access the web service. Open the `UPSPackageTrackerWeb\web.config` file and enter them now.

```XML
<configuration>
  <appSettings>
    <add key="UPSSecurityUsernameToken.Username" value="[enter your username]" />
    <add key="UPSSecurityUsernameToken.Password" value="[enter your password]" />
    <add key="UPSSecurityServiceAccessToken.AccessLicenseNumber" value="[enter your access license number]" />
  </appSettings>
  <system.web>
```

Download the UPS developer kit zip file and extract it to your computer. 

The UPS developer kit provides a WSDL file which was used for adding the service reference to the UPSPackageTrackerWeb project.  Adding the service reference automatically adds an endpoint address into the web.config file which references the *test* UPS service. Use the commented addresses in the web.config file to switch back and forth between test and production as needed.
  
### 1.2. Server-side API ###

The server-side API calls the UPS tracking service for a single tracking number, caches the result and returns a JSON response.  The file UPSPackageTrackerWeb\Controllers\UPSTrackingController.cs contains our simplified JSON API for calling back into the UPS tracking service. The code initializes a connection to Azure table storage, checks if a cached version of the tracking response exists and calls the UPS tracking service if needed.

The address for calling the API will be in the format `/api/UPSTracking/1ZE680080304005492`  

### 1.3. App for Office Manifest ###

The UPSPackageTrackerManifest defines the connection between the Outlook and the app web pages hosted within Outlook. The key configuration is the Read Form Activation rule which indicates that the message body must contain text matching the regular expression `(1Z\w{16})`, which is a simplified pattern for a UPS Ground tracking number.

> Note: a more complicated regular expression is likely needed to match all of the different types of UPS tracking numbers. 

The completed Read Form manifest page:
![](http://i.imgur.com/T8CGcBa.png)

### 1.4. Front end code ###

The manifest file specifies a source location which will be loaded when the app loads within Outlook. In our case this is `UPSPackageTrackerWeb/AppRead/Home/Home.html`. The `Home.html` is a simple HTML file containing all the user interface elements needed to render the app.

The heavy lifting is performed by `Home.js` file located in the same directory. Key elements in the JavaScript file include:

-  Extracting the regular expression matches:
```JavaScript
var item = Office.cast.item.toItemRead(Office.context.mailbox.item);
var matches = item.getRegExMatchesByName("UPSTrackingNumberInBody");
```
-  Calling our JSON tracking API:
```JavaScript
 $.getJSON("../../api/UPSTracking/" + trackingNumber, {}, function (data) { ... });
```
- Adding tracking data to the results table:
```JavaScript
$('#trackingTable > tbody:last').append($("<tr>" +
    "<td>" + addressStr +
    "</td><td>" + activity.dateField.splice(4, 0, "-").splice(7, 0, "-") + " " + activity.timeField.splice(2, 0, ":").splice(5, 0, ":") +
    "</td><td>" + activity.statusField.descriptionField + "</td><tr>"));
```

## 2. Running the Solution ##

The Azure Storage emulator must be started before running the solution. A simple method for starting development storage for Azure is to open Server Explorer, expand Azure, then Storage, then Development.

Once the storage emulator has started, press F5 to run the project. 