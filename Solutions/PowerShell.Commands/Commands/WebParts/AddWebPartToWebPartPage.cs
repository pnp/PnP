using System.IO;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using File = System.IO.File;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOWebPartToWebPartPage")]
    [CmdletHelp("Adds a webpart to a web part page in a specified zone", Category = "Web Parts")]
    [CmdletExample(
   Code = @"PS:> Add-SPOWebPartToWebPartPage -PageUrl ""/sites/demo/sitepages/home.aspx"" -Path ""c:\myfiles\listview.webpart"" -ZoneId ""Header"" -ZoneIndex 1 ",
   Remarks = @"This will add the webpart as defined by the XML in the listview.webpart file to the specified page in the specified zone and with the order index of 1", SortOrder = 1)]
    [CmdletExample(
  Code = @"PS:> Add-SPOWebPartToWebPartPage -PageUrl ""/sites/demo/sitepages/home.aspx"" -XML $webpart -ZoneId ""Header"" -ZoneIndex 1 ",
  Remarks = @"This will add the webpart as defined by the XML in the $webpart variable to the specified page in the specified zone and with the order index of 1", SortOrder = 1)]
    public class AddWebPartToWebPartPage : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Server Relative Url of the page to add the webpart to.")]
        public string PageUrl = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "XML", HelpMessage = "A string containing the XML for the webpart.")]
        public string Xml = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "FILE", HelpMessage = "A path to a webpart file on a the file system.")]
        public string Path = string.Empty;

        [Parameter(Mandatory = true)]
        public string ZoneId;

        [Parameter(Mandatory = true)]
        public int ZoneIndex;

        protected override void ExecuteCmdlet()
        {
           

            WebPartEntity wp = null;

            switch (ParameterSetName)
            {
                case "FILE":
                    if (!System.IO.Path.IsPathRooted(Path))
                    {
                        Path = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Path);
                    }

                    if (File.Exists(Path))
                    {
                        var fileStream = new StreamReader(Path);
                        var webPartString = fileStream.ReadToEnd();
                        fileStream.Close();

                        wp = new WebPartEntity {WebPartZone = ZoneId, WebPartIndex = ZoneIndex, WebPartXml = webPartString};
                    }
                    break;
                case "XML":
                    wp = new WebPartEntity {WebPartZone = ZoneId, WebPartIndex = ZoneIndex, WebPartXml = Xml};
                    break;
            }
            if (wp != null)
            {
                SelectedWeb.AddWebPartToWebPartPage(PageUrl, wp);
            }
        }
    }
}
