using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Management.Automation;
using OfficeDevPnP.SPOnline.Core;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPONavigationLink")]
    public class AddNavigationLink : SPOWebCmdlet
    {

        [Parameter(Mandatory = true)]
        public SPOWeb.NavigationNodeType Location;

        [Parameter(Mandatory = true)]
        public string Title;

        [Parameter(Mandatory = false)]
        public string Url;

        [Parameter(Mandatory = false)]
        public SwitchParameter AsLast;

        [Parameter(Mandatory = false)]
        public string Header;

        [Parameter(Mandatory = false)]
        public string Previous;

        protected override void ExecuteCmdlet()
        {
            if(Url == null)
            {
                ClientContext.Load(this.SelectedWeb, w => w.Url);
                ClientContext.ExecuteQuery();
                Url = this.SelectedWeb.Url;
            }
            SPOWeb.AddNavigationLink(this.SelectedWeb, Location, Title, Url, AsLast, Header, Previous, ClientContext);
        }
    }

    
}
