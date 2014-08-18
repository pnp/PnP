using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOTenantSite", ConfirmImpact = ConfirmImpact.High)]
    [CmdletHelp("Office365 only: Removes a site collection from the current tenant", DetailedDescription = @"

You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command. 
", Details = "Office365 only")]
    public class RemoveSite : SPOAdminCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Url;

        [Parameter(Mandatory = false, HelpMessage = "Do not add to the trashcan if selected.")]
        [Alias("SkipTrash")]
        public SwitchParameter SkipRecycleBin;

        [Parameter(Mandatory = false, HelpMessage = "If specified, will wait for the site to be deleted, otherwise the deletion will happen asynchronously.")]
        public SwitchParameter Wait;

        [Parameter(Mandatory = false, HelpMessage = "If specified, will search for the site in the Recycle Bin and remove it from there.")]
        public SwitchParameter FromRecycleBin;


        [Parameter(Mandatory = false, HelpMessage = "Do not ask for confirmation.")]
        public SwitchParameter Force;

        protected override void ProcessRecord()
        {
            if (Force || ShouldContinue(string.Format(Properties.Resources.RemoveSiteCollection0, Url), Properties.Resources.Confirm))
            {
                if (FromRecycleBin)
                {
                    SPOnline.Core.SPOSite.DeleteTenantSiteFromRecycleBin(Url, Tenant, Wait);
                }
                else
                {
                    SPOnline.Core.SPOSite.DeleteTenantSite(Url, Tenant, Wait, SkipRecycleBin);
                }

            }
        }

    }
}
