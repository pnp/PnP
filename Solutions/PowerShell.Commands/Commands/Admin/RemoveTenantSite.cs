#if !CLIENTSDKV15
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Resources = OfficeDevPnP.PowerShell.Commands.Properties.Resources;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOTenantSite", ConfirmImpact = ConfirmImpact.High)]
    [CmdletHelp("Office365 only: Removes a site collection from the current tenant", DetailedDescription = @"

You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command. 
", Details = "Office365 only")]
    public class RemoveSite : SPOAdminCmdlet
    {
        [Parameter(Mandatory = true, Position=0, ValueFromPipeline=true)]
        public string Url;

        [Parameter(Mandatory = false, HelpMessage = "Do not add to the trashcan if selected.")]
        [Alias("SkipTrash")]
        public SwitchParameter SkipRecycleBin;

        [Parameter(Mandatory = false, HelpMessage = "OBSOLETE: If true, will wait for the site to be deleted before processing continues", DontShow=true)]
        public SwitchParameter Wait;

        [Parameter(Mandatory = false, HelpMessage = "If specified, will search for the site in the Recycle Bin and remove it from there.")]
        public SwitchParameter FromRecycleBin;


        [Parameter(Mandatory = false, HelpMessage = "Do not ask for confirmation.")]
        public SwitchParameter Force;

        protected override void ProcessRecord()
        {
            if (Force || ShouldContinue(string.Format(Resources.RemoveSiteCollection0, Url), Resources.Confirm))
            {
                if (!FromRecycleBin)
                {
                    Tenant.DeleteSiteCollection(Url, !SkipRecycleBin);
                }
                else
                {
                    Tenant.DeleteSiteCollectionFromRecycleBin(Url);
                }
            }
        }

    }
}
#endif