#if !CLIENTSDKV15
using System.Management.Automation;
using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOTenantSite")]
    [CmdletHelp(@"Office365 only: Uses the tenant API to set site information.", Details = "", Category = "Tenant Administration")]
    public class SetTenantSite : SPOAdminCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The URL of the site", Position=0, ValueFromPipeline=true)]
        public string Url;

        [Parameter(Mandatory = false)]
        public string Title;
        [Parameter(Mandatory = false)]
        public SharingCapabilities? Sharing = null;

        [Parameter(Mandatory = false)]
        public long? StorageMaximumLevel = null;

        [Parameter(Mandatory = false)]
        public long? StorageWarningLevel = null;

        [Parameter(Mandatory = false)]
        public double? UserCodeMaximumLevel = null;

        [Parameter(Mandatory = false)]
        public double? UserCodeWarningLevel = null;

        [Parameter(Mandatory = false)]
        public SwitchParameter? AllowSelfServiceUpgrade = null;

        protected override void ExecuteCmdlet()
        {
            Tenant.SetSiteProperties(Url, title:Title, sharingCapability: Sharing, storageMaximumLevel: StorageMaximumLevel, allowSelfServiceUpgrade: AllowSelfServiceUpgrade, userCodeMaximumLevel: UserCodeMaximumLevel, userCodeWarningLevel: UserCodeWarningLevel);
        }
    }

}
#endif