using System.Linq;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOWebPartProperty")]
    [CmdletHelp("Returns a web part property", Category = "Web Parts")]
    public class GetWebPartProperty : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string PageUrl = string.Empty;

        [Parameter(Mandatory = true)]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = false)]
        public string Key;

        protected override void ExecuteCmdlet()
        {
            var properties = SelectedWeb.GetWebPartProperties(Identity.Id, PageUrl);
            var values = properties.FieldValues.Select(x => new PropertyBagValue() { Key = x.Key, Value = x.Value });
            if (!string.IsNullOrEmpty(Key))
            {
                var value = values.FirstOrDefault(v => v.Key == Key);
                if (value != null)
                {
                    WriteObject(value.Value);
                }
            }
            else
            {
                WriteObject(values, true);
            }
        }



    }
}
