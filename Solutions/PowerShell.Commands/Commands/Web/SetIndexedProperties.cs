using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOIndexedProperties")]
    public class SetIndexedProperties : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public List<string> Keys;

        protected override void ExecuteCmdlet()
        {
            if (Keys != null && Keys.Count > 0)
            {
                SelectedWeb.RemovePropertyBagValue("vti_indexedpropertykeys");

                foreach (var key in Keys)
                {
                    SelectedWeb.AddIndexedPropertyBagKey(key);
                }
            }
        }
    }
}
