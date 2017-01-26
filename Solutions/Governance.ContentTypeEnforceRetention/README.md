# SharePoint Online Content Type Retention #

### Summary ###
In SharePoint Online you cannot enforce all items to have retention policies applied. End users can create content types and not apply retention if they don't want to. In a governed scenario, this might be a problem - the ILM team cannot be confident that all items have retention as required.

At the time of writing this, the Client Side Object Model does not have methods to set retention on content types (only on sites). This solution is an example of a few things:

- Recursive 'crawling' of webs
- Running queries for each document library in a web, where items are filtered by ContentTypeId and Last Modified Date
- Authentication with add-in only tokens and execution across many site collections
- The use of Throttling of the requests

This solution is a console application designed to run periodically as a scheduled task or Azure WebJob. It will crawl all items in given site collections and find old items by last modified date.

One important part is the use of Core.Throttling - this is crucial for any scenario where recursive crawling/loading is applied.

The logic is configurable by Content Type, you can specify the ContentTypeId and days of validity. The tool will scan each library for a content type match, where the last modified date is greater than the validity specified in the configuration. All found items can be processed (deleted, workflow started, whatever is needed).

The demonstration shows how you can run a console program periodically, without any user interaction. In a similiar way the console applicaiton could be replaced with a windows service to perform unattended operations to your tenant data. The code logic makes use of the add-in only policy to perform calls "without any user". In my example I have used the Tenant permission scope with FullControl rights.  

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
The complete solution requires add-in registration with tenant permissions

### Solution ###
Solution | Author(s)
---------|----------
Governance.ContentTypeEnforceRetention | Evgeni Petrov (OneBit Software) & Radi Atanassov (OneBit Software)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | January 17th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# Overview #
The solution has one Visual Studio project - Governance.ContentTypeEnforceRetention, which is a console application. It is intended that this application is to be ran as a scheduled task or a Azure WebJob, however it could be easily replaced with a windows service if you prefer that design.

One interesting aspect of the demonstration is that there is no "SharePoint add-in" project, no add-in package file. It uses the add-in registration pages to acquire a ClientId and ClientSecret, but there is no deployment to the tenant.

# Setup and Execution #
This solution is kept as simple as possible to illustrate the key elements. There is no add-in package, you just need to register a ClientId and ClientSecret and provide appropriate permissions.

## Dependencies ##
The project has references to the following assemblies:

- Microsoft.SharePoint.Client.dll
- Microsoft.SharePoint.Client.Runtime.dll
- System.Configuration (part of the framework, used to read appSettings in the app.config file)

## Permission Configuration ##
This solution uses a provider-hosted approach, but does not have a tradition SharePoint add-in entry point and will execute periodically with no user interaction (ie - Add-In Only). Because of these two constraints, you must register the add-in in /_layouts/15/appregnew.aspx and then manually configure permissions for the add-in in /_layouts/15/appinv.aspx:

Permission Request XML:

```XML
<AppPermissionRequests AllowAppOnlyPolicy="true">
    <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl" />
</AppPermissionRequests>
```
Start by getting and registering a ClientId and ClientSecret through /_layouts/15/AppRegNew.aspx

![Appregnew.aspx UI](http://i.imgur.com/pEoS3qJ.png)

Make sure you copy the ClientId and ClientSecret and plug them in the app.config. Notice how the add-in Domain and Redirect URI are not required in this case beacuse we have no URL interaction

When done you will get a confirmation message:

![Add-in registration details](http://i.imgur.com/pDVzYQk.png)

Continue by giving it permissions through /_layouts/15/AppInv.aspx

![AppInv page UI](http://i.imgur.com/2GyqHtH.png)

You will then be asked to confirm the trust and assign the add-in permissions:

![Trust concent](http://i.imgur.com/ZvgCKXl.png)


## Application Settings ##
The solution requires some configuration in the app.config file.
The follow code sample outlines the appSettings that need to be configured with values specific to your tenant/environment.

"Sites" is a key/value collection of Site Collection URL's to scan
"ContentTypeRetentionPolicyPeriod" contains key/value pairs of ContentTypeId and the duration in days of the validity of an item of the specific content type.

```XML
<appSettings>
    <!-- The client id and client secret of the add-in as provided in /_layouts/15/appregnew.aspx -->
    <add key="ClientID" value="enter id" />
    <add key="ClientSecret" value="enter secret" />
  </appSettings>

  <Sites> <!--Site Collections to scan-->
    <add key="site1" value="https://[tenant].sharepoint.com/sites/contoso"/>
    <add key="site2" value="https://[tenant].sharepoint.com/sites/contosobeta"/>
  </Sites>

  <ContentTypeRetentionPolicyPeriod>
    <!--Key is the Content Type ID and Value is the days of validity-->
    <add key="0x0101009148F5A04DDD49cbA7127AADA5FB792B006973ACD696DC4858A76371B2FB2F439A" value="183" /> <!--Audio-->
    <add key="0x0101" value="365" /> <!--Document-->
  </ContentTypeRetentionPolicyPeriod>
```

<img src="https://telemetry.sharepointpnp.com/pnp/solutions/Governance.ContentTypeEnforceRetention" />