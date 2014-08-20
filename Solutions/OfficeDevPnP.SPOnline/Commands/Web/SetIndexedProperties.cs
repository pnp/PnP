using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
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
                this.SelectedWeb.RemovePropertyBagValue("vti_indexedpropertykeys");

                foreach (var key in Keys)
                {
                    this.SelectedWeb.AddIndexedPropertyBagKey(key);
                }
            }
        }
    }
}
