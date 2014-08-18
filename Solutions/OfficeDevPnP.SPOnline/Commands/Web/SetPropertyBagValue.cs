using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOPropertyBagValue")]
    public class SetPropertyBagValue : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Key;

        [Parameter(Mandatory = true)]
        public string Value;

        protected override void ExecuteCmdlet()
        {

            SPOnline.Core.SPOWeb.SetPropertyBagValue(Key, Value, this.SelectedWeb, ClientContext);
        }
    }
}
