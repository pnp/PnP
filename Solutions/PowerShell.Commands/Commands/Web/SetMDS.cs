using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using System;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOMinimalDownloadStrategy")]
    [CmdletHelp("Activates or deactivates the minimal downloading strategy.")]
    public class SetMDS : SPOWebCmdlet
    {
        [Parameter(ParameterSetName = "On", Mandatory = true)]
        public SwitchParameter On;

        [Parameter(ParameterSetName = "Off", Mandatory = true)]
        public SwitchParameter Off;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (On)
            {
                SelectedWeb.Features.Add(new Guid(Properties.Resources.MDSFeatureGuid), Force, FeatureDefinitionScope.None);
            }
            else
            {
                SelectedWeb.Features.Remove(new Guid(Properties.Resources.MDSFeatureGuid), Force);
            }
            ClientContext.ExecuteQueryRetry();
        }
    }

}
