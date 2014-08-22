using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.Core;

namespace OfficeDevPnP.PowerShell.Commands
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
