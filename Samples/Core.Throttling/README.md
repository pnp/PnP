---
page_type: sample
products:
- office-sp
languages:
- aspx
extensions:
  contentType: samples
  technologies:
  - Add-ins
  createdDate: 1/1/2016 12:00:00 AM
---
# SharePoint Online Throttling #

Please use following guidance for ensuring proper handling of the CSOM Throttling in SharePoint Online.

* [Avoid getting throttled or blocked in SharePoint Online](https://docs.microsoft.com/en-us/sharepoint/dev/general-development/how-to-avoid-getting-throttled-or-blocked-in-sharepoint-online)
* Please use [PnP CSOM Core extension](https://www.nuget.org/packages/SharePointPnPCoreOnline/) to automatically handle throttling based on Microsoft guidance
  * This code is available from NuGet gallery - after you have referenced it, use 'ExecuteQueryRetry' for your CSOM executions - this will automatically handle the throttling for you. This code is open-source and available from [GitHub](https://github.com/pnp/PnP-Sites-Core/blob/master/Core/OfficeDevPnP.Core/Extensions/ClientContextExtensions.cs#L88)

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.Throttling" />