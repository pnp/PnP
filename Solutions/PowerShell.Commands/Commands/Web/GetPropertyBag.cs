using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOPropertyBag")]
    public class GetPropertyBag : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, Position=0, ValueFromPipeline=true)]
        public string Key = string.Empty;
        protected override void ExecuteCmdlet()
        {
            if (!string.IsNullOrEmpty(Key))
            {
                WriteObject(this.SelectedWeb.GetPropertyBagValueString(Key, string.Empty));
            }
            else
            {
                if (this.SelectedWeb.IsPropertyAvailable("AllProperties"))
                {
                    WriteObject(SelectedWeb.AllProperties.FieldValues);
                }
                else
                {
                    PropertyValues values = this.SelectedWeb.AllProperties;
                    ClientContext.Load(values);
                    ClientContext.ExecuteQuery();
                    WriteObject(values.FieldValues);
                }
            }
        }
    }
}
