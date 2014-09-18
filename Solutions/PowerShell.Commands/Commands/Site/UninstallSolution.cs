using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsLifecycle.Uninstall, "SPOSolution")]
    [CmdletHelp("Uninstalls a sandboxed solution from a site collection")]
    public class UninstallSolution : SPOCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage="ID of the solution, from the solution manifest")]
        public GuidPipeBind PackageId;

        [Parameter(Mandatory = true, HelpMessage="Filename of the WSP file to uninstall")]
        public string PackageName;

        [Parameter(Mandatory = false, HelpMessage = "Optional major version of the solution, defaults to 1")]
        public int MajorVersion = 1;

        [Parameter(Mandatory = false, HelpMessage = "Optional minor version of the solution, defaults to 0")]
        public int MinorVersion = 0;

        protected override void ExecuteCmdlet()
        {
            ClientContext.Site.UninstallSolution(PackageId.Id, PackageName, MajorVersion, MinorVersion);
        }
    }
}
