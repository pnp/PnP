using OfficeDevPnP.SPOnline.Commands.Base;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using OfficeDevPnP.SPOnline.CmdletHelpAttributes;

namespace OfficeDevPnP.SPOnline.Commands.Features
{
    [Cmdlet("Enable", "SPOFeature")]
    [CmdletHelp("Enables a feature")]
    [CmdletExample(Code = "PS:> Enable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe")]
    [CmdletExample(Code = "PS:> Enable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Force")]
    [CmdletExample(Code = "PS:> Enable-SPOnlineFeature -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe -Scope Web")]
    public class EnableFeature : SPOCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The id of the feature to enable.")]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = false, HelpMessage = "Forcibly enable the feature.")]
        public SwitchParameter Force;

        [Parameter(Mandatory = false)]
        public OfficeDevPnP.SPOnline.Core.SPOFeatures.FeatureScope Scope = OfficeDevPnP.SPOnline.Core.SPOFeatures.FeatureScope.Web;


        protected override void ExecuteCmdlet()
        {
            Guid featureId = Identity.Id;
            if(Scope == Core.SPOFeatures.FeatureScope.Web)
            {
                ClientContext.Web.ActivateFeature(featureId);
            }
            else
            {
                ClientContext.Site.ActivateFeature(featureId);
            }
        }


    }
}
