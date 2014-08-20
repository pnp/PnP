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

        [Parameter(Mandatory = false)]
        public SwitchParameter Indexed;

        protected override void ExecuteCmdlet()
        {

            this.SelectedWeb.SetPropertyBagValue(Key, Value);
            if(Indexed)
            {
                this.SelectedWeb.AddIndexedPropertyBagKey(Key);
            }
            else
            {
                this.SelectedWeb.RemoveIndexedPropertyBagKey(Key);
            }
        }
    }
}
