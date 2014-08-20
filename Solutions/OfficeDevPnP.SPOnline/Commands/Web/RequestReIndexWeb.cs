using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;
using OfficeDevPnP.SPOnline.Core;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsLifecycle.Request, "SPOReIndexWeb")]
    public class RequestReIndexWeb : SPOWebCmdlet
    {

        protected override void ExecuteCmdlet()
        {
            this.SelectedWeb.ReIndexSite();
        }
    }
}
