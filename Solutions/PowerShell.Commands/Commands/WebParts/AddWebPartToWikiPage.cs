using System.IO;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using File = System.IO.File;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOWebPartToWikiPage")]
    [CmdletHelp("Adds a webpart to a wiki page in a specified table row and column", Category = "Web Parts")]
    [CmdletExample(
Code = @"PS:> Add-SPOWebPartToWikiPage -PageUrl ""/sites/demo/sitepages/home.aspx"" -Path ""c:\myfiles\listview.webpart"" -Row 1 -Column 1",
Remarks = @"This will add the webpart as defined by the XML in the listview.webpart file to the specified page in the first row and the first column of the HTML table present on the page", SortOrder = 1)]
    [CmdletExample(
  Code = @"PS:> Add-SPOWebPartToWikiPage -PageUrl ""/sites/demo/sitepages/home.aspx"" -XML $webpart -Row 1 -Column 1",
  Remarks = @"This will add the webpart as defined by the XML in the $webpart variable to the specified page in the first row and the first column of the HTML table present on the page", SortOrder = 1)]
    public class AddWebPartToWikiPage : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string PageUrl = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "XML")]
        public string Xml = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "FILE")]
        public string Path = string.Empty;

        [Parameter(Mandatory = true)]
        public int Row;

        [Parameter(Mandatory = true)]
        public int Column;

        [Parameter(Mandatory = false)]
        public SwitchParameter AddSpace;

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

                        wp = new WebPartEntity { WebPartXml = webPartString };
                    }
                    break;
                case "XML":
                    wp = new WebPartEntity { WebPartXml = Xml };
                    break;
            }
            if (wp != null)
            {
                SelectedWeb.AddWebPartToWikiPage(PageUrl, wp, Row, Column, AddSpace);
            }
        }
    }
}
