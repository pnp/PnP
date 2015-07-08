using System.Linq;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOPropertyBag")]
    [CmdletHelp("Returns the property bag values.", Category = "Webs")]
    public class GetPropertyBag : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true)]
        public string Key = string.Empty;
        protected override void ExecuteCmdlet()
        {
            if (!string.IsNullOrEmpty(Key))
            {
                WriteObject(SelectedWeb.GetPropertyBagValueString(Key, string.Empty));
            }
            else
            {
                if (!SelectedWeb.IsPropertyAvailable("AllProperties"))
                {
                    ClientContext.Load(SelectedWeb.AllProperties);
                    ClientContext.ExecuteQueryRetry();

                }
                var values = SelectedWeb.AllProperties.FieldValues.Select(x => new PropertyBagValue() { Key = x.Key, Value = x.Value });
                WriteObject(values, true);
            }
        }
    }

    public class PropertyBagValue
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}
