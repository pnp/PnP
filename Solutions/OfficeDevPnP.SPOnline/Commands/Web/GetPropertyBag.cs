using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOPropertyBag")]
    public class GetPropertyBag : SPOWebCmdlet
    {
        protected override void ExecuteCmdlet()
        {
            WriteObject(SPOnline.Core.SPOWeb.GetPropertyBag(this.SelectedWeb, ClientContext));
        }
    }
}
