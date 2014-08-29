using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

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
