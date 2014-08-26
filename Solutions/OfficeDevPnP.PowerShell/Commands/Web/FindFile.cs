using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Find, "SPOFile")]
    [CmdletHelp("Finds a file in the virtual file system of the web.")]
    [CmdletExample(Code = @"
PS:> Find-SPOFile -Match *.master
", Remarks = "Will return all masterpages located in the current web.")]
    public class FindFile : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Wildcard query", ValueFromPipeline = true)]
        public string Match = string.Empty;

        protected override void ExecuteCmdlet()
        {
            WriteObject(PowerShell.Core.SPOWeb.FindFiles(this.SelectedWeb, Match, ClientContext), true);
        }
    }
}
