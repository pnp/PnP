using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Base;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOWebTemplates")]
    [CmdletHelp(@"Office365 only: Returns the available web templates

You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command. 
")]
    [CmdletExample(
        Code = @"PS:> Get-SPOWebTemplates")]
    [CmdletExample(
        Code = @"PS:> Get-SPOWebTemplates -LCID 1033",
        Remarks = @"Returns all webtemplates for the Locale with ID 1033 (English)")]

    public class GetWebTemplates : SPOAdminCmdlet
    {
        [Parameter(Mandatory = false)]
        public uint LCID;

        [Parameter(Mandatory = false)]
        public int CompatibilityLevel;

        protected override void ProcessRecord()
        {
            WriteObject(ClientContext.Web.GetWebTemplatesTenant(LCID, CompatibilityLevel));
        }
    }
}
