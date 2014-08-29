using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPONavigationNode")]
    public class AddNavigationNode : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage="Either 'Top' or 'Quicklaunch'")]
        public NavigationNodeType Location;

        [Parameter(Mandatory = true)]
        public string Title;

        [Parameter(Mandatory = false)]
        public string Url;

        [Parameter(Mandatory = false)]
        public string Header;

        protected override void ExecuteCmdlet()
        {
            if(Url == null)
            {
                ClientContext.Load(this.SelectedWeb, w => w.Url);
                ClientContext.ExecuteQuery();
                Url = this.SelectedWeb.Url;
            }
            this.SelectedWeb.AddNavigationNode(Title, new Uri(Url), Header, Location == NavigationNodeType.QuickLaunch ? true : false);
        }

        public enum NavigationNodeType
        {
            Top,
            QuickLaunch
        }
    }

    
}
