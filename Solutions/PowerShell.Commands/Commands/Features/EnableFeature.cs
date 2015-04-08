using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using OfficeDevPnP.PowerShell.Commands.Enums;

namespace OfficeDevPnP.PowerShell.Commands.Features
{
    [Cmdlet("Enable", "SPOFeature")]
    [CmdletHelp("Enables a feature", Category = "Features")]
    [CmdletExample(Code = "PS:> Enable-SPOFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe")]
    [CmdletExample(Code = "PS:> Enable-SPOFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Force")]
    [CmdletExample(Code = "PS:> Enable-SPOFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Scope Web")]
    public class EnableFeature : SPOCmdlet
    {
        [Parameter(Mandatory = true, Position=0, ValueFromPipeline=true, HelpMessage = "The id of the feature to enable.")]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = false, HelpMessage = "Forcibly enable the feature.")]
        public SwitchParameter Force;

        [Parameter(Mandatory = false)]
        public FeatureScope Scope = FeatureScope.Web;

        [Parameter(Mandatory = false, HelpMessage = "Specify this parameter if the feature you're trying to active is part of a sandboxed solution.")]
        public SwitchParameter Sandboxed;


        protected override void ExecuteCmdlet()
        {
            var featureId = Identity.Id;
            if(Scope == FeatureScope.Web)
            {
                ClientContext.Web.ActivateFeature(featureId, Sandboxed);
            }
            else
            {
                ClientContext.Site.ActivateFeature(featureId, Sandboxed);
            }
        }

    }
}
