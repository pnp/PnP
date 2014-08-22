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
using OfficeDevPnP.PowerShell.Core;

namespace OfficeDevPnP.PowerShell.Commands.Features
{
    [Cmdlet(VerbsCommon.Get, "SPOFeature")]
    [CmdletHelp("Gets all features")]
    public class GetFeature : SPOCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The scope of the feature. Defaults to Web.")]
        public SPOFeatures.FeatureScope Scope = SPOFeatures.FeatureScope.Web;

        protected override void ExecuteCmdlet()
        {
            WriteObject(PowerShell.Core.SPOFeatures.GetFeatures(Scope, ClientContext));
        }


    }
}
