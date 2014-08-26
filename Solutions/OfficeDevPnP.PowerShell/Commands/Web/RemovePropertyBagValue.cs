using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.Core;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOPropertyBagValue", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemovePropertyBagValue : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Key;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {

            if (this.SelectedWeb.PropertyBagContainsKey(Key))
            {
                if (Force || ShouldContinue(string.Format(Properties.Resources.Delete0, Key), Properties.Resources.Confirm))
                {
                    this.SelectedWeb.RemovePropertyBagValue(Key);
                }
            }
        }
    }
}
