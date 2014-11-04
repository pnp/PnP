using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using System;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOFolder")]
    [CmdletHelp("Creates a folder within a parent folder")]
    [CmdletExample(Code = @"
PS:> Add-SPOFolder -Name NewFolder -Folder _catalogs/masterpage/newfolder")]
    public class AddFolder : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The folder name")]
        public string Name = string.Empty;

        [Parameter(Mandatory = true, HelpMessage = "The parent folder in the site")]
        public string Folder = string.Empty;

        protected override void ExecuteCmdlet()
        {
            if (!this.SelectedWeb.IsPropertyAvailable("ServerRelativeUrl"))
            {
                ClientContext.Load(this.SelectedWeb, w => w.ServerRelativeUrl);
                ClientContext.ExecuteQuery();
            }

            Folder folder = this.SelectedWeb.GetFolderByServerRelativeUrl(UrlUtility.Combine(this.SelectedWeb.ServerRelativeUrl, Folder));
            ClientContext.Load(folder, f => f.ServerRelativeUrl);
            ClientContext.ExecuteQuery();

            folder.CreateFolder(Name);
        }
    }
}
