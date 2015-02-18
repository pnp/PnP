using System.IO;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using File = System.IO.File;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOWebPartToWebPartPage")]
    public class AddWebPartToWebPartPage : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string PageUrl = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "XML")]
        public string Xml = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "FILE")]
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
