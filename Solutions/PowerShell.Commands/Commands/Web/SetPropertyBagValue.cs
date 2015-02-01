using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
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
            if (!Indexed)
            {
                // If it is already an indexed property we still have to add it back to the indexed properties
                Indexed = !string.IsNullOrEmpty(SelectedWeb.GetIndexedPropertyBagKeys().FirstOrDefault(k => k == Key));
            }

            SelectedWeb.SetPropertyBagValue(Key, Value);
            if(Indexed)
            {
                SelectedWeb.AddIndexedPropertyBagKey(Key);
            }
            else
            {
                SelectedWeb.RemoveIndexedPropertyBagKey(Key);
            }
        }
    }
}
