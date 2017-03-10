# Simple Event Registration Form using Knockout.js #

### Summary ###
This sample shows how to implement a simple dynamic form using Knockout.js. In the sample standard Event display form is extended to enable registrations.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None.

### Solution ###
Solution | Author(s)
---------|----------
Core.KnockoutJSForm | Antons Mislevics (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | August 6th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------


# Overview #
This sample demonstrates customizing standard SharePoint forms by using Knockout.js. It extends standard display form in Events list to allow users registering for events. 

![Presentation of list view with custom UI](http://i.imgur.com/qHWv4Y8.png)

This is achieved by applying the following configurations to standard Team site:

1. Events list is created to store information about upcoming events. Standard Calendar List template is used.
2. Events list is extended by boolean field "Registration Allowed". This is set to true in order to allow users registering for specific event.
3. Event Registration list is created from Custom List template.
4. Event Registration list is extended by Lookup column Event, that links registrations to events.
5. Indexes on Author and Event columns are configured in Event Registration list.
6. Cascading delete behavior is configured for Event column in Event Registration list.
7. Custom UI component that allows users to register/unregister for the event is implemented using Knockout.js. It consists of two files:
  - event-registration-form-template.js - implements UI of the component;
  - event-registration-form.js - implements data model;
8. JavaScript files are deployed to Style Library;
9. UI component is embedded on display form of the Events list via Content Editor web part with ContentLink pointing to event-registration-form-template.js.

# Running the sample #
The sample is implemented as a console application that automates deployment. The following steps must be completed in order to run the sample:

1. Create new site collection based on standard Team site template.
2. Open Visual Studio project and update values for `siteUrl`, `username` and `password` variables in `Program.cs`.
3. Run the project.
4. Check registration functionality in Events list.

# Implementation details #
You should note the following implementation details:

- Knockout.js form implementation is split into two files in order to clearly separate UI from data model.
- Additional variables are introduced in data model, in order to track loading status of the form. It allows to show smooth "Loading..." message, when the form is querying data from SharePoint, instead of half-loaded UI.
- JavaScript and Web Part files included in the project contain "~sitecollection" token that is replaced by server relative  URL during provisioning. This is required in order to enable smooth deployment in separate site collections for test and production environments. This also enables deploying the sample on sub sites.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.KnockoutJSForm" />