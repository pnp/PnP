using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOWebPart")]
    public class AddWebPart : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "FILETOWIKI")]
        [Parameter(Mandatory = true, ParameterSetName = "FILETOZONE")]
        public string Path = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "PARTTOZONE")]
        public WebPart WebPart = null;

        [Parameter(Mandatory = true, ParameterSetName = "FILETOWIKI")]
        [Parameter(Mandatory = true, ParameterSetName = "XMLTOWIKI")]
        [Parameter(Mandatory = true, ParameterSetName = "PARTTOZONE")]
        [Parameter(Mandatory = true, ParameterSetName = "FILETOZONE")]
        public string PageUrl = string.Empty;

        [Parameter(Mandatory = false, ParameterSetName = "PARTTOZONE")]
        [Parameter(Mandatory = true, ParameterSetName = "FILETOZONE")]
        public string ZoneId;

        [Parameter(Mandatory = false, ParameterSetName = "PARTTOZONE")]
        [Parameter(Mandatory = true, ParameterSetName = "FILETOZONE")]
        public int ZoneIndex;

        [Parameter(Mandatory = false, ParameterSetName = "FILETOWIKI")]
        [Parameter(Mandatory = false, ParameterSetName = "XMLTOWIKI", Position = 2)]
        public int Row = 0;

        [Parameter(Mandatory = false, ParameterSetName = "FILETOWIKI")]
        [Parameter(Mandatory = false, ParameterSetName = "XMLTOWIKI", Position = 3)]
        public int Column = 0;

        [Parameter(Mandatory = false, ParameterSetName = "XMLTOWIKI", Position = 1)]
        public string Xml = string.Empty;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "FILETOWIKI")
            {

                if (System.IO.File.Exists(Path))
                {
                    System.IO.StreamReader fileStream = new System.IO.StreamReader(Path);
                    string webPartString = fileStream.ReadToEnd();
                    fileStream.Close();

                    if (!string.IsNullOrEmpty(ZoneId))
                    {
                        PowerShell.Core.SPOWebParts.ImportWebPart(webPartString, PageUrl, ZoneId, ZoneIndex, this.SelectedWeb, ClientContext);
                    }
                    else
                    {
                        PowerShell.Core.SPOWebParts.AddWebPart(webPartString, this.SelectedWeb, PageUrl, ClientContext, Row, Column);
                    }
                }
            }
            else if (ParameterSetName == "FILETOZONE")
            {
                if (System.IO.File.Exists(Path))
                {
                    System.IO.StreamReader fileStream = new System.IO.StreamReader(Path);
                    string webPartString = fileStream.ReadToEnd();
                    fileStream.Close();
                    PowerShell.Core.SPOWebParts.ImportWebPart(webPartString, PageUrl, ZoneId, ZoneIndex, this.SelectedWeb, ClientContext);
                }

            }
            else if (ParameterSetName == "PARTTOZONE")
            {
                PowerShell.Core.SPOWebParts.AddWebPart(WebPart, PageUrl, ZoneId, ZoneIndex, this.SelectedWeb, ClientContext);
            }
            else if (ParameterSetName == "XMLTOWIKI")
            {
                PowerShell.Core.SPOWebParts.AddWebPart(Xml, this.SelectedWeb, PageUrl, ClientContext, Row, Column);
            }
        }
    }
}
