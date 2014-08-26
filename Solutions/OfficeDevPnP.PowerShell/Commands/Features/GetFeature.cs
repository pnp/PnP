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
    public class GetFeature : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The scope of the feature. Defaults to Web.")]
        public FeatureScope Scope = FeatureScope.Web;

        protected override void ExecuteCmdlet()
        {
            List<Feature> features = new List<Feature>();
            FeatureCollection featureCollection = null;
            if (Scope == FeatureScope.Site)
            {
                featureCollection = ClientContext.Site.Features;
            }
            else
            {
                featureCollection = this.SelectedWeb.Features;
            }

            var query = ClientContext.LoadQuery(featureCollection);
            ClientContext.ExecuteQuery();
            WriteObject(query);
        }


        public enum FeatureScope
        {
            Web,
            Site
        }
    }
}
