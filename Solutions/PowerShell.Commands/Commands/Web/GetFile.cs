using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System;
using System.IO;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOFile")]
    [CmdletHelp("Downloads a file.")]
    [CmdletExample(Code = @"
PS:> Get-SPOFile -ServerRelativeUrl /sites/project/_catalogs/themes/15/company.spcolor",Remarks="Downloads the file and saves it to the current folder", SortOrder = 1)]
    [CmdletExample(Code = @"
PS:> Get-SPOFile -ServerRelativeUrl /sites/project/_catalogs/themes/15/company.spcolor -Path c:\temp -FileName company.spcolor", Remarks="Downloads the file and saves it to c:\\temp\\company.spcolor",SortOrder = 2)]
    [CmdletExample(Code = @"
PS:> Get-SPOFile -ServerRelativeUrl /sites/project/_catalogs/themes/15/company.spcolor -AsString", Remarks = "Downloads the file and outputs its contents to the console", SortOrder = 3)]
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
                this.SelectedWeb.SaveFileToLocal(ServerRelativeUrl, Path, Filename);
            }
            else
            {
                WriteObject(this.SelectedWeb.GetFileAsString(ServerRelativeUrl));
            }

        }
    }
}
