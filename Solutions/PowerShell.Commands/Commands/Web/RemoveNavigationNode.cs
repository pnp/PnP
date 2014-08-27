using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.Core;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPONavigationNode")]
    public class RemoveNavigationNode : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage="Either 'Top' or 'Quicklaunch'")]
        public SPOWeb.NavigationNodeType Location;

        [Parameter(Mandatory = true)]
        public string Title;

        [Parameter(Mandatory = false)]
        public string Header;

        [Parameter(Mandatory = false)]
        public SwitchParameter QuickLaunch = true;

        protected override void ExecuteCmdlet()
        {
            this.SelectedWeb.DeleteNavigationNode(Title, Header, Location == SPOWeb.NavigationNodeType.QuickLaunch ? true : false);
        }
    }

    
}
