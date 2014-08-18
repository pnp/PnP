using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using System.Management.Automation;
using SPO = OfficeDevPnP.SPOnline.Core;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOTenantSite")]
    [CmdletHelp(@"Office365 only: Uses the tenant API to retrieve site information.

You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command. 
", Details = "Requires a connection to a SharePoint Tenant Admin site.")]
    [CmdletExample(Code = @"
PS:> Get-SPOTenantSite", Remarks = "Returns all site collections")]
    [CmdletExample(Code = @"
PS:> Get-SPOTenantSite -Identity http://tenant.sharepoint.com/sites/projects", Remarks = "Returns information about the project site.")]
    public class GetTenantSite : SPOAdminCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The URL of the site")]
        public string Identity;

        [Parameter(Mandatory = false)]
        public SwitchParameter Detailed;

        protected override void ExecuteCmdlet()
        {
            if (SPOnlineConnection.CurrentConnection.ConnectionType == SPOnlineConnection.ConnectionTypes.OnPrem)
            {
                WriteObject(SPO.SPOSite.GetSite(ClientContext));
            }
            else
            {
                if (!string.IsNullOrEmpty(Identity))
                {
                    WriteObject(SPOnline.Core.SPOSite.GetTenantSitePropertiesByUrl(Tenant, Identity, Detailed));
                }
                else
                {
                    WriteObject(SPOnline.Core.SPOSite.GetTenantSiteProperties(Tenant, Detailed));
                }
            }
        }
    }

}
