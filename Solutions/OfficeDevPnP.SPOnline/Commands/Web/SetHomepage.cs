using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOHomePage")]
    [CmdletHelp("Sets the home page of the current web.")]
    [CmdletExample(
        Code = @"
    PS:> Set-SPOHomePage -Path SitePages/Home.aspx
",
        Remarks = "Sets the home page to the home.aspx file which resides in the SitePages library")]
    public class SetHomePage : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The root folder relative path of the homepage")]
        public string Path = string.Empty;

        protected override void ExecuteCmdlet()
        {
            SPOnline.Core.SPOWeb.SetHomePage(Path, this.SelectedWeb, ClientContext);
        }
    }

}
