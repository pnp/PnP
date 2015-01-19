using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System;
using System.Linq;
using OfficeDevPnP.PowerShell.Commands.Enums;

namespace OfficeDevPnP.PowerShell.Commands.Features
{
    [Cmdlet(VerbsCommon.Get, "SPOFeature")]
    [CmdletHelp("Returns all or a specific feature")]
    [CmdletExample(
     Code = @"PS:> Get-SPOFeature",
     Remarks = @"This will return all web scoped features", SortOrder = 1)]
    [CmdletExample(
     Code = @"PS:> Get-SPOFeature -Scope Site",
     Remarks = @"This will return all site scoped features", SortOrder = 2)]
    [CmdletExample(
     Code = @"PS:> Get-SPOFeature -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22",
     Remarks = @"This will return a specific web scoped feature", SortOrder = 3)]
    [CmdletExample(
     Code = @"PS:> Get-SPOFeature -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22 -Scope Site",
     Remarks = @"This will return a specific site scoped feature", SortOrder = 3)]
    public class GetFeature : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true)]
        public FeaturePipeBind Identity;

        [Parameter(Mandatory = false, HelpMessage = "The scope of the feature. Defaults to Web.")]
        public FeatureScope Scope = FeatureScope.Web;

        protected override void ExecuteCmdlet()
        {
            var features = new List<Feature>();
            FeatureCollection featureCollection = null;
            if (Scope == FeatureScope.Site)
            {
                featureCollection = ClientContext.Site.Features;
            }
            else
            {
                featureCollection = SelectedWeb.Features;
            }
            IEnumerable<Feature> query = null;
#if !CLIENTSDKV15
            if (ClientContext.ServerVersion.Major > 15)
            {
                query = ClientContext.LoadQuery(featureCollection.IncludeWithDefaultProperties(f => f.DisplayName));
            }
            else
            {
                query = ClientContext.LoadQuery(featureCollection.IncludeWithDefaultProperties());
            }
#else
            query = ClientContext.LoadQuery(featureCollection.IncludeWithDefaultProperties());
#endif
            ClientContext.ExecuteQuery();
            if (Identity == null)
            {
                WriteObject(query, true);
            }
            else
            {
                if (Identity.Id != Guid.Empty)
                {
                    WriteObject(query.Where(f => f.DefinitionId == Identity.Id));
                }
                else if (!string.IsNullOrEmpty(Identity.Name))
                {
#if !CLIENTSDKV15
                    WriteObject(query.Where(f => f.DisplayName.Equals(Identity.Name, StringComparison.OrdinalIgnoreCase)));
#else
                    throw new Exception("Querying by name is not supported in version 15 of the Client Side Object Model");
#endif
                }
            }
        }

    }
}
