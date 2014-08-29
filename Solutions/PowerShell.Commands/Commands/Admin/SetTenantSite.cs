using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOTenantSite")]
    [CmdletHelp(@"Office365 only: Uses the tenant API to set site information.

You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command. 
", Details = "Requires a connection to a SharePoint Tenant Admin site.")]
    public class SetTenantSite : SPOAdminCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The URL of the site")]
        public string Url;

        [Parameter(Mandatory = false)]
        public string Title;
        [Parameter(Mandatory = false)]
        public Nullable<Microsoft.Online.SharePoint.TenantManagement.SharingCapabilities> Sharing = null;

        [Parameter(Mandatory = false)]
        public Nullable<long> StorageMaximumLevel = null;

        [Parameter(Mandatory = false)]
        public Nullable<long> StorageWarningLevel = null;

        [Parameter(Mandatory = false)]
        public Nullable<double> UserCodeMaximumLevel = null;

        [Parameter(Mandatory = false)]
        public Nullable<double> UserCodeWarningLevel = null;

        [Parameter(Mandatory = false)]
        public Nullable<SwitchParameter> AllowSelfServiceUpgrade = null;

        protected override void ExecuteCmdlet()
        {
            this.Tenant.SetSiteProperties(Url, title:Title, sharingCapability: Sharing, storageMaximumLevel: StorageMaximumLevel, allowSelfServiceUpgrade: AllowSelfServiceUpgrade, userCodeMaximumLevel: UserCodeMaximumLevel, userCodeWarningLevel: UserCodeWarningLevel);
        }
    }

}


