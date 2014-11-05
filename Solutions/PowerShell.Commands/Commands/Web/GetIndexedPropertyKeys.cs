using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOIndexedPropertyKeys")]
    public class GetIndexedProperties : SPOWebCmdlet
    {
      
        protected override void ExecuteCmdlet()
        {
            var keys = this.SelectedWeb.GetIndexedPropertyBagKeys();
            WriteObject(keys);
        }
    }
}
