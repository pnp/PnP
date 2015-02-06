using System;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using File = System.IO.File;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsLifecycle.Install, "SPOSolution")]
    [CmdletHelp("Installs a sandboxed solution to a site collection")]
    public class InstallSolution : SPOCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage="ID of the solution, from the solution manifest")]
        public GuidPipeBind PackageId;

        [Parameter(Mandatory = true, HelpMessage="Path to the sandbox solution package (.WSP) file")]
        public string SourceFilePath;

        [Parameter(Mandatory = false, HelpMessage="Optional major version of the solution, defaults to 1")]
        public int MajorVersion = 1;

        [Parameter(Mandatory = false, HelpMessage="Optional minor version of the solution, defaults to 0")]
        public int MinorVersion = 0;

        protected override void ExecuteCmdlet()
        {
            if (File.Exists(SourceFilePath))
            {
                ClientContext.Site.InstallSolution(PackageId.Id, SourceFilePath, MajorVersion, MinorVersion);
            }
            else
            {
                throw new Exception("File does not exist");
            }
        }
    }
}
