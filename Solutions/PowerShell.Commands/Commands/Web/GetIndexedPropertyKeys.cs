using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOIndexedPropertyKeys")]
    [CmdletHelp("Returns the keys of the property bag values that have been marked for indexing by search", Category = "Webs")]
    public class GetIndexedProperties : SPOWebCmdlet
    {
        protected override void ExecuteCmdlet()
        {
            var keys = SelectedWeb.GetIndexedPropertyBagKeys();
            WriteObject(keys);
        }
    }
}
