#if !CLIENTSDKV15
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Base;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOWebTemplates")]
    [CmdletHelp(@"Office365 only: Returns the available web templates.", Category = "Tenant Administration")]
    [CmdletExample(
        Code = @"PS:> Get-SPOWebTemplates", SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Get-SPOWebTemplates -LCID 1033",
        Remarks = @"Returns all webtemplates for the Locale with ID 1033 (English)", SortOrder = 2)]

    public class GetWebTemplates : SPOAdminCmdlet
    {
        [Parameter(Mandatory = false)]
        public uint Lcid;

        [Parameter(Mandatory = false)]
        public int CompatibilityLevel;

        protected override void ProcessRecord()
        {
            WriteObject(Tenant.GetWebTemplates(Lcid, CompatibilityLevel));
        }
    }
}
#endif