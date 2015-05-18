#New-SPOTenantSite
*Topic automatically generated on: 2015-04-29*

Office365 only: Creates a new site collection for the current tenant
##Syntax
```powershell
New-SPOTenantSite -Title <String> -Url <String> [-Description <String>] [-Owner <String>] [-Lcid <UInt32>] [-Template <String>] -TimeZone <Int32> [-ResourceQuota <Double>] [-ResourceQuotaWarningLevel <Double>] [-StorageQuota <Int64>] [-StorageQuotaWarningLevel <Int64>] [-RemoveDeletedSite [<SwitchParameter>]] [-Wait [<SwitchParameter>]]```
&nbsp;

##Detailed Description

The New-SPOTenantSite cmdlet creates a new site collection for the current company. However, creating a new SharePoint
Online site collection fails if a deleted site with the same URL exists in the Recycle Bin.

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Description|String|False|
Lcid|UInt32|False|Specifies the language of this site collection. For more information, see Locale IDs Assigned by Microsoft
(http://go.microsoft.com/fwlink/p/?LinkId=242911) (http://go.microsoft.com/fwlink/p/?LinkId=242911).
Owner|String|False|Specifies the user name of the site collection's primary owner. The owner must be a user instead of a security
group or an email-enabled security group.
RemoveDeletedSite|SwitchParameter|False|
ResourceQuota|Double|False|Specifies the quota for this site collection in Sandboxed Solutions units. This value must not exceed the
company's aggregate available Sandboxed Solutions quota. The default value is 0. For more information, see
Resource Usage Limits on Sandboxed Solutions in SharePoint
2010(http://msdn.microsoft.com/en-us/library/gg615462.aspx)
(http://msdn.microsoft.com/en-us/library/gg615462.aspx).
ResourceQuotaWarningLevel|Double|False|
StorageQuota|Int64|False|Specifies the storage quota for this site collection in megabytes. This value must not exceed the company's
available quota.

StorageQuotaWarningLevel|Int64|False|
Template|String|False|Specifies the site collection template type. Use the Get-SPOWebTemplate cmdlet to get the list of valid
templates. If no template is specified, one can be added later. The Template and LocaleId parameters must be a
valid combination as returned from the Get-SPOnlineWebTemplate cmdlet.
TimeZone|Int32|True|Use Get-SPOnlineTimeZone to retrieve possible timezone values
Title|String|True|
Url|String|True|Specifies the full URL of the new site collection. It must be in a valid managed path in the company's site.
For example, for company contoso, valid managed paths are https://contoso.sharepoint.com/sites and
https://contoso.sharepoint.com/teams.
Wait|SwitchParameter|False|
<!-- Ref: 8C640D03D33A93E71242897F087112DB -->