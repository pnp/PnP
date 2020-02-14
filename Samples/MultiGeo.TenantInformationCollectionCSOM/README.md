---
page_type: sample
products:
- office-sp
languages:
- csharp
extensions:
  contentType: samples
  technologies:
  - Add-ins
  createdDate: 3/1/2018 12:00:00 AM
---
# MultiGeo.TenantInformationCollection #

### Summary ###
This sample shows how you can use the CSOM API to obtain information about the geo locations in a Multi-Geo tenant.


### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
none

### Solution ###
Solution | Author(s)
---------|----------
MultiGeo.TenantInformationCollectionCSOM | Bert Jansen (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | February 28th 2017 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

## Application setup steps
Open `program.cs` of the sample and insert the URL of your tenant admin center + user name:

```C#
string tenantAdminUrl = "https://contoso-admin.sharepoint.com";
string userName = "admin@contoso.onmicrosoft.com";
```

## Learning more about this sample
This sample is part of a guidance series that explains developers how they can prepare their applications for use in Multi-Geo tenants. Checkout https://aka.ms/multigeo-developer-guidance to learn more about developing for a Multi-Geo tenant.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/MultiGeo.TenantInformationCollectionCSOM" />