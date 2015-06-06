using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOSitePolicy")]
    [CmdletHelp("Retrieves all or a specific site policy", Category = "Information Management")]
    [CmdletExample(
     Code = @"PS:> Get-SPOSitePolicy",
     Remarks = @"Retrieves the current applied site policy.", SortOrder = 1)]
    [CmdletExample(
     Code = @"PS:> Get-SPOSitePolicy -AllAvailable",
     Remarks = @"Retrieves all available site policies.", SortOrder = 2)]
    [CmdletExample(
      Code = @"PS:> Get-SPOSitePolicy -Name ""Contoso HBI""",
      Remarks = @"Retrieves an available site policy with the name ""Contoso HBI"".", SortOrder = 3)]

    public class GetSitePolicy : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "Retrieve all available site policies")]
        public SwitchParameter AllAvailable;

        [Parameter(Mandatory = false, HelpMessage = "Retrieves a site policy with a specific name")]
        public string Name;

        protected override void ExecuteCmdlet()
        {

            if (!this.MyInvocation.BoundParameters.ContainsKey("AllAvailable") && !this.MyInvocation.BoundParameters.ContainsKey("Name"))
            {
                // Return the current applied site policy
                WriteObject(this.SelectedWeb.GetAppliedSitePolicy());
            }
            else
            {
                if (this.MyInvocation.BoundParameters.ContainsKey("AllAvailable"))
                {
                    WriteObject(this.SelectedWeb.GetSitePolicies(),true);
                    return;
                }
                if (this.MyInvocation.BoundParameters.ContainsKey("Name"))
                {
                    WriteObject(this.SelectedWeb.GetSitePolicyByName(Name));
                    return;
                }
                
            }
        }

    }

}


