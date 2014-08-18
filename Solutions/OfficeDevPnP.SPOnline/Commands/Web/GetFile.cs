using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using Microsoft.SharePoint.Client;
using System;
using System.IO;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOFile")]
    [CmdletHelp("Downloads a file.")]
    [CmdletExample(Code = @"
PS:> Get-SPOFile ")]
    public class GetFile : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "FILE")]
        [Parameter(Mandatory = true, ParameterSetName = "STRING")]
        public string ServerRelativeUrl = string.Empty;

        [Parameter(Mandatory = false, ParameterSetName = "FILE")]
        public string Path = string.Empty;

        [Parameter(Mandatory = false, ParameterSetName = "FILE")]
        public string Filename = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "STRING")]
        public SwitchParameter AsString;

        protected override void ExecuteCmdlet()
        {

            if (ParameterSetName == "FILE")
            {
                if (string.IsNullOrEmpty(Path))
                {
                    Path = Directory.GetCurrentDirectory();
                }
                SPOnline.Core.SPOWeb.GetFile(ServerRelativeUrl, Path, Filename, this.SelectedWeb, ClientContext);
            }
            else
            {
                WriteObject(SPOnline.Core.SPOWeb.GetFile(ServerRelativeUrl, this.SelectedWeb, ClientContext));
            }

        }
    }
}
