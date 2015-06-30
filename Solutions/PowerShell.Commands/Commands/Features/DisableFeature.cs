using System;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using OfficeDevPnP.PowerShell.Commands.Enums;

namespace OfficeDevPnP.PowerShell.Commands.Features
{
    [Cmdlet("Disable", "SPOFeature", SupportsShouldProcess = false)]
    [CmdletHelp("Disables a feature", Category = "Features")]
    [CmdletExample(Code = "PS:> Disable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe", SortOrder = 1)]
    [CmdletExample(Code = "PS:> Disable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Force", SortOrder = 2)]
    [CmdletExample(Code = "PS:> Disable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Scope Web", SortOrder = 3)]
    public class DisableFeature : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = ParameterAttribute.AllParameterSets, HelpMessage = "The id of the feature to disable.")]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets, HelpMessage = "Forcibly disable the feature.")]
        public SwitchParameter Force;

        [Parameter(Mandatory = false, HelpMessage = "Specify the scope of the feature to deactivate, either Web or Site. Defaults to Web.")]
        public FeatureScope Scope = FeatureScope.Web;

        protected override void ExecuteCmdlet()
        {
            Guid featureId = Identity.Id;

            if (Scope == FeatureScope.Web)
            {
                this.SelectedWeb.DeactivateFeature(featureId);
            }
            else
            {
                ClientContext.Site.DeactivateFeature(featureId);
            }
        }
    }
}
