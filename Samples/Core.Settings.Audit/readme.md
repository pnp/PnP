# Control auditing settings in site collection using CSOM #

### Summary ###
Demonstrates how to control auditing settings in teh site collection level usign CSOM.

### Applies to ###
-  Office 365 Multi Tenant (MT) - Waiting for new redistributable
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
Any special pre-requisites?

### Solution ###
Solution | Author(s)
---------|----------
Core.Settings.Audit | Vesa Juvonen

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | December 22th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Doc scenario 1 #
Draft. Will be updated shortly.


## Sub level 1.1 ##
Description:
Code snippet:
```C#
string scenario1Page = String.Format("scenario1-{0}.aspx", DateTime.Now.Ticks);
string scenario1PageUrl = csomService.AddWikiPage("Site Pages", scenario1Page);
```



