# Control auditing settings in site collection using CSOM #

### Summary ###
Demonstrates how to control auditing settings in the site collection level using CSOM.

### Applies to ###
-  Office 365 Multi Tenant (MT) 
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
2014 December CU installed on farm for on-premises or new re-distributable package for cloud CSOM (April 2015 release).

### Solution ###
Solution | Author(s)
---------|----------
Core.Settings.Audit | Vesa Juvonen

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | December 22th 2014 | Initial release
1.1  | April 13th 2015 | Updated to use latest Office 365 CSOM

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #
Sample shows how to control the auditing settings in the site collection level using client side object model.

![Add-in UI](http://i.imgur.com/oYakX68.png)

## Enabling auditing in site collection level ##
Following code example shows how to enable all auditing setting in site collection level and how to adjust audit log trimming.
```C#
Microsoft.SharePoint.Client.Site site = clientContext.Site;
Audit audit = site.Audit;
clientContext.Load(site);
clientContext.Load(audit);
clientContext.ExecuteQuery();
// Enable all auditing is site collection level
site.Audit.AuditFlags = Microsoft.SharePoint.Client.AuditMaskType.All;
site.Audit.Update();
// Adjust retention time to be 7 days
site.AuditLogTrimmingRetention = 7;
site.TrimAuditLog = true;
clientContext.ExecuteQuery();
```
## Disabling auditing in site collection level ##
Following code example shows how to disable auditing from site collection level.
```C#
Microsoft.SharePoint.Client.Site site = clientContext.Site;
Audit audit = site.Audit;
clientContext.Load(site);
clientContext.Load(audit);
clientContext.ExecuteQuery();
// Set remove any auditing from site colelction level
site.Audit.AuditFlags = Microsoft.SharePoint.Client.AuditMaskType.None;
site.Audit.Update();
site.TrimAuditLog = false;
clientContext.ExecuteQuery();
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.Settings.Audit" />