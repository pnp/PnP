#if !CLIENTSDKV15
using System.Linq;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Enums;
using Resources = OfficeDevPnP.PowerShell.Commands.Properties.Resources;

namespace OfficeDevPnP.PowerShell.Commands
{

    [Cmdlet(VerbsCommon.Get, "SPOTenantSite", SupportsShouldProcess = true)]
    [CmdletHelp(@"Office365 only: Uses the tenant API to retrieve site information.

You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command. 
", Details = "Requires a connection to a SharePoint Tenant Admin site.")]
    [CmdletExample(Code = @"
PS:> Get-SPOTenantSite", Remarks = "Returns all site collections")]
    [CmdletExample(Code = @"
PS:> Get-SPOTenantSite -Identity http://tenant.sharepoint.com/sites/projects", Remarks = "Returns information about the project site.")]
    public class GetTenantSite : SPOAdminCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The URL of the site", Position = 0, ValueFromPipeline = true)]
        [Alias("Identity")]
        public string Url;

        [Parameter(Mandatory = false)]
        public SwitchParameter Detailed;

        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeOneDriveSites;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (SPOnlineConnection.CurrentConnection.ConnectionType == ConnectionType.OnPrem)
            {
                WriteObject(ClientContext.Site);
            }
            else
            {
                if (!string.IsNullOrEmpty(Url))
                {
                    var list = Tenant.GetSitePropertiesByUrl(Url, Detailed);
                    list.Context.Load(list);
                    list.Context.ExecuteQueryRetry();
                    WriteObject(list, true);
                }
                else
                {
                    var list = Tenant.GetSiteProperties(0, Detailed);
                    list.Context.Load(list);
                    list.Context.ExecuteQueryRetry();
                    var siteProperties = list.ToList();
                    if (IncludeOneDriveSites)
                    {
                        if (Force || ShouldContinue(Resources.GetTenantSite_ExecuteCmdlet_This_request_can_take_a_long_time_to_execute__Continue_, Resources.Confirm))
                        {
                            var onedriveSites = Tenant.GetOneDriveSiteCollections();

                            var personalUrl = ClientContext.Url.ToLower().Replace("-admin", "-my");
                            foreach (var site in onedriveSites)
                            {
                                var siteprops = Tenant.GetSitePropertiesByUrl(string.Format("{0}/{1}", personalUrl.TrimEnd('/'), site.Url.Trim('/')), Detailed);
                                ClientContext.Load(siteprops);
                                ClientContext.ExecuteQueryRetry();
                                siteProperties.Add(siteprops);
                            }
                        }
                    }
                    WriteObject(siteProperties.OrderBy(x => x.Url), true);
                }
            }
        }
    }

}
#endif