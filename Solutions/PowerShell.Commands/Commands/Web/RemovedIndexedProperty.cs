using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOIndexedProperty")]
    [CmdletHelp("Removes a key from propertybag to be indexed by search. The key and it's value retain in the propertybag, however it will not be indexed anymore.", Category = "Webs")]
    public class RemovedIndexedProperty : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, Position = 0)]
        public string Key;

        protected override void ExecuteCmdlet()
        {
            if (!string.IsNullOrEmpty(Key))
            {
                SelectedWeb.RemoveIndexedPropertyBagKey(Key);
            }
        }
    }
}
