# Modernization.PageTransformation #

### Summary ###

This code sample demonstrates how to use the preview version of the SharePoint Modernization library to modernize classic wiki and web part pages into modern client side pages.

> **Important**
> This sample is using a first preview release of the SharePoint Modernization library. If you encounter issues please log them in https://github.com/SharePoint/PnP-Tools/issues

### Resources ###

- [Modernization guidance overview on docs.microsoft.com](https://aka.ms/sppnp-modernize)
- [Page transformation guidance](https://docs.microsoft.com/en-us/sharepoint/dev/transform/modernize-userinterface-site-pages)
- [YouTube recording from May 3rd SIG call containing a page migration demo](https://youtu.be/Uf2f8ISBpVg?t=15m31s)

### Applies to ###

- SharePoint Online

### Prerequisites ###

None

### Solution ###

Solution | Author(s)
---------|----------
Modernization.PageTransformation | Bert Jansen (**Microsoft**)

### Version history ###

Version  | Date | Comments
---------| -----| --------
1.3  | November 7th 2018 | Upgraded to latest modernization framework version
1.2  | September 25th 2018 | Upgraded to first production release of the PnP Modernization framework
1.1  | August 3rd 2018 | Upgraded to beta release of the PnP Modernization framework
1.0  | May 15nd 2018 | Initial release

### Disclaimer ###

**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

## Getting started ##

To use this sample you'll need to make 2 small changes to `program.cs`: replace the `siteUrl` and `userName` variables with a site and user from your environment.

```c#
string siteUrl = "https://contoso.sharepoint.com/sites/mytestsite";
string userName = "joe@contoso.onmicrosoft.com";
```

Once that's done, press F5 to use the sample.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Modernization.PageTransformation" />
