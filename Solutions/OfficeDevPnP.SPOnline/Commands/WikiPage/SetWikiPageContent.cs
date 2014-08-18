using OfficeDevPnP.SPOnline.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using Microsoft.SharePoint.Client.WebParts;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOWikiPageContent")]
    public class SetWikiPageContent : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "STRING")]
        public string Content = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "FILE")]
        public string Path = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "FILE")]
        [Parameter(Mandatory = true, ParameterSetName = "STRING")]
        public string PageUrl = string.Empty;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "FILE")
            {
                if (System.IO.File.Exists(Path))
                {
                    System.IO.StreamReader fileStream = new System.IO.StreamReader(Path);
                    string contentString = fileStream.ReadToEnd();
                    fileStream.Close();

                    SPOnline.Core.SPOWikiPage.SetWikiPageContent(PageUrl, contentString, this.SelectedWeb, ClientContext);

                }
            }
            else
            {
                SPOnline.Core.SPOWikiPage.SetWikiPageContent(PageUrl, Content, this.SelectedWeb, ClientContext);
            }
        }
    }
}
