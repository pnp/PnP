using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOIndexedPropertyKeys")]
    public class GetIndexedProperties : SPOWebCmdlet
    {
        protected override void ExecuteCmdlet()
        {
            var keys = SelectedWeb.GetIndexedPropertyBagKeys();
            WriteObject(keys);
        }
    }
}
