using System.Linq;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.InformationPolicy;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOSitePolicy")]
    [CmdletHelp("Sets a site policy", Category = "Information Management")]
    [CmdletExample(
      Code = @"PS:> Set-SPOSitePolicy -Name ""Contoso HBI""",
      Remarks = @"This applies a site policy with the name ""Contoso HBI"" to the current site. The policy needs to be available in the site.", SortOrder = 1)]
    public class ApplySitePolicy : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The name of the site policy to apply")]
        public string Name;

       
        protected override void ExecuteCmdlet()
        {
            SelectedWeb.ApplySitePolicy(Name);
        }
    }
}


