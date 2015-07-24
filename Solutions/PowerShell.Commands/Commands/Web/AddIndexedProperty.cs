using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOIndexedProperty")]
    [CmdletHelp("Marks the value of the propertybag key to be indexed by search.", Category = "Webs")]
    public class AddIndexedProperty : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Key;

        protected override void ExecuteCmdlet()
        {
            if (!string.IsNullOrEmpty(Key))
            {
                SelectedWeb.AddIndexedPropertyBagKey(Key);
            }
        }
    }
}
