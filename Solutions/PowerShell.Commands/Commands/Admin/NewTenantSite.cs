using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.New, "SPOTenantSite")]
    [CmdletHelp("Office365 only: Creates a new site collection for the current tenant", DetailedDescription = @"
The New-SPOTenantSite cmdlet creates a new site collection for the current company. However, creating a new SharePoint
Online site collection fails if a deleted site with the same URL exists in the Recycle Bin.

You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command. 
", Details = "Office365 only")]
    public class NewTenantSite : SPOAdminCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Title;

        [Parameter(Mandatory = true, HelpMessage = @"Specifies the full URL of the new site collection. It must be in a valid managed path in the company's site.
For example, for company contoso, valid managed paths are https://contoso.sharepoint.com/sites and
https://contoso.sharepoint.com/teams.")]
        public string Url;

        [Parameter(Mandatory = false)]
        public string Description = string.Empty;

        [Parameter(Mandatory = false, HelpMessage = @"Specifies the user name of the site collection's primary owner. The owner must be a user instead of a security
group or an email-enabled security group.")]
        public string Owner = string.Empty;

        [Parameter(Mandatory = false, HelpMessage = @"Specifies the language of this site collection. For more information, see Locale IDs Assigned by Microsoft
(http://go.microsoft.com/fwlink/p/?LinkId=242911) (http://go.microsoft.com/fwlink/p/?LinkId=242911).")]
        public uint Lcid = 1033;

        [Parameter(Mandatory = false, HelpMessage = @"Specifies the site collection template type. Use the Get-SPOWebTemplate cmdlet to get the list of valid
templates. If no template is specified, one can be added later. The Template and LocaleId parameters must be a
valid combination as returned from the Get-SPOnlineWebTemplate cmdlet.")]
        public string Template = "STS#0";

        [Parameter(Mandatory = true, HelpMessage = "Use Get-SPOnlineTimeZone to retrieve possible timezone values")]
        public int TimeZone;

        [Parameter(Mandatory = false, HelpMessage = @"Specifies the quota for this site collection in Sandboxed Solutions units. This value must not exceed the
company's aggregate available Sandboxed Solutions quota. The default value is 0. For more information, see
Resource Usage Limits on Sandboxed Solutions in SharePoint
2010(http://msdn.microsoft.com/en-us/library/gg615462.aspx)
(http://msdn.microsoft.com/en-us/library/gg615462.aspx).")]
        public double ResourceQuota = 0;

        [Parameter(Mandatory = false)]
        public double ResourceQuotaWarningLevel = 0;

        [Parameter(Mandatory = false, HelpMessage = @"Specifies the storage quota for this site collection in megabytes. This value must not exceed the company's
available quota.
")]
        public long StorageQuota = 100;

        [Parameter(Mandatory = false)]
        public long StorageQuotaWarningLevel = 100;

        [Parameter(Mandatory = false)]
        public SwitchParameter RemoveDeletedSite;

        [Parameter(Mandatory = false)]
        public SwitchParameter Wait;

        protected override void ProcessRecord()
        {
            ClientContext.Web.CreateSiteCollectionTenant(Url, Title, Owner, Template, (int)StorageQuota, (int)StorageQuotaWarningLevel, TimeZone, (int)ResourceQuota, (int)ResourceQuotaWarningLevel, Lcid,RemoveDeletedSite,Wait);
        }

    }
}
