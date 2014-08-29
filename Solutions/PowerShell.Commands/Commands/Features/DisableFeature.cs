using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands.Features
{
    [Cmdlet("Disable", "SPOFeature", SupportsShouldProcess = false)]
    [CmdletHelp("Disables a feature")]
    [CmdletExample(Code = "PS:> Disable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe")]
    [CmdletExample(Code = "PS:> Disable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Force")]
    [CmdletExample(Code = "PS:> Disable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Scope Web")]
    public class DisableFeature : SPOCmdlet
    {
        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets, HelpMessage = "The id of the feature to disable.")]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets, HelpMessage = "Forcibly disable the feature.")]
        public SwitchParameter Force;

        [Parameter(Mandatory = false)]
        public FeatureScope Scope = FeatureScope.Web;

        protected override void ExecuteCmdlet()
        {
            Guid featureId = Identity.Id;

            if (Scope == FeatureScope.Web)
            {
                ClientContext.Web.DeactivateFeature(featureId);
            }
            else
            {
                ClientContext.Site.DeactivateFeature(featureId);
            }
        }

        public enum FeatureScope
        {
            Web,
            Site
        }
    }
}
