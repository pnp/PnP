# MultiGeo.SiteEnumeration #

### Summary ###
This sample shows how you can use the Microsoft Graph API and the SharePoint CSOM tenant API's to enumerate site collections across all the geo locations in a Multi-Geo tenant.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
none

### Solution ###
Solution | Author(s)
---------|----------
MultiGeo.SiteEnumeration | Bert Jansen (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | September 25th 2017 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

## Application setup steps
Before you can use this sample application you'll need to register an Azure AD application and grant it the needed permissions. Please checkout https://aka.ms/multigeo-developer-samplesetup to get a step by step explanation of the needed setup steps. After following the described steps you'll have three pieces of information that you need to plug into the sample application:
- Azure AD Application ID (e.g. 7b51701e-f118-4447-bc86-a39a8fe7d2b1)
- Azure AD Application Password (e.g. 9ogfh0ERgyr8XJXcKplKZLe)
- Azure AD domain (e.g. contoso.onmicrosoft.com)

Open `program.cs` of the sample and adjust the following line based on the above three values:

```C#
MultiGeoManager multiGeoManager = new MultiGeoManager("<application id>", "<application password>", "<Azure AD domain>");
```

Since this application also uses the CSOM tenant admin API you'll need to replace below values:
```C#
string siteUrl = "https://<your tenant>.sharepoint.com"; //e.g. https://contoso.sharepoint.com
string userName = "<your user>@<your tenant>.onmicrosoft.com"; //e.g. bob@contoso.onmicrosoft.com
```

## Learning more about this sample
This sample is part of a guidance series that explains developers how they can prepare their applications for use in Multi-Geo tenants. Checkout https://aka.ms/multigeo-developer-guidance to learn more about developing for a Multi-Geo tenant.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/MultiGeo.SiteEnumeration" />