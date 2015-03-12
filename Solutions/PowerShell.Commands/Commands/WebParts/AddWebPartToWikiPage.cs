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
                    if (File.Exists(Path))
                    {
                        var fileStream = new StreamReader(Path);
                        var webPartString = fileStream.ReadToEnd();
                        fileStream.Close();

                        wp = new WebPartEntity {WebPartXml = webPartString};
                    }
                    break;
                case "XML":
                    wp = new WebPartEntity {WebPartXml = Xml};
                    break;
            }
            if (wp != null)
            {
                SelectedWeb.AddWebPartToWikiPage(PageUrl, wp, Row, Column, AddSpace);
            }
        }
    }
}
