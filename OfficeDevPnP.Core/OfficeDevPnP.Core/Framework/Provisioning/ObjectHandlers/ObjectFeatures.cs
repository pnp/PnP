using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using Feature = OfficeDevPnP.Core.Framework.Provisioning.Model.Feature;
using System;
using System.Linq;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectFeatures : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "Features"; }
        }

        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_Features);

            var context = web.Context as ClientContext;

            // if this is a sub site then we're not enabling the site collection scoped features
            if (!web.IsSubSite())
            {
                var siteFeatures = template.Features.SiteFeatures;
                ProvisionFeaturesImplementation<Site>(context.Site, siteFeatures);
            }

            var webFeatures = template.Features.WebFeatures;
            ProvisionFeaturesImplementation<Web>(web, webFeatures);
        }

        private static void ProvisionFeaturesImplementation<T>(T parent, List<Feature> features)
        {
            var activeFeatures = new List<Microsoft.SharePoint.Client.Feature>();
            Web web = null;
            Site site = null;
            if (parent is Site)
            {
                site = parent as Site;
                site.Context.Load(site.Features, fs => fs.Include(f => f.DefinitionId));
                site.Context.ExecuteQueryRetry();
                activeFeatures = site.Features.ToList();
            }
            else
            {
                web = parent as Web;
                web.Context.Load(web.Features, fs => fs.Include(f => f.DefinitionId));
                web.Context.ExecuteQueryRetry();
                activeFeatures = web.Features.ToList();
            }

            if (features != null)
            {
                foreach (var feature in features)
                {
                    try
                    {

                        if (!feature.Deactivate)
                        {
                            if (activeFeatures.FirstOrDefault(f => f.DefinitionId == feature.Id) == null)
                            {
                                if (site != null)
                                {
                                    site.ActivateFeature(feature.Id);
                                }
                                else
                                {
                                    web.ActivateFeature(feature.Id);
                                }
                            }

                        }
                        else
                        {
                            if (activeFeatures.FirstOrDefault(f => f.DefinitionId == feature.Id) != null)
                            {
                                if (site != null)
                                {
                                    site.DeactivateFeature(feature.Id);
                                }
                                else
                                {
                                    web.DeactivateFeature(feature.Id);
                                }
                            }
                        }
                    }
                    catch (ServerException serverEx)
                    {
                        Log.Error(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING,
                            CoreResources.Provisioning_ObjectHandlers_Features, serverEx.Message);
                    }
                }
            }
        }


        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            var context = web.Context as ClientContext;
            bool isSubSite = web.IsSubSite();
            var webFeatures = web.Features;
            var siteFeatures = context.Site.Features;

            context.Load(webFeatures, fs => fs.Include(f => f.DefinitionId));
            if (!isSubSite)
            {
                context.Load(siteFeatures, fs => fs.Include(f => f.DefinitionId));
            }
            context.ExecuteQueryRetry();

            var features = new Features();
            foreach (var feature in webFeatures)
            {
                features.WebFeatures.Add(new Feature() { Deactivate = false, Id = feature.DefinitionId });
            }

            // if this is a sub site then we're not creating  site collection scoped feature entities
            if (!isSubSite)
            {
                foreach (var feature in siteFeatures)
                {
                    features.SiteFeatures.Add(new Feature() { Deactivate = false, Id = feature.DefinitionId });
                }
            }

            template.Features = features;

            // If a base template is specified then use that one to "cleanup" the generated template model
            if (creationInfo.BaseTemplate != null)
            {
                template = CleanupEntities(template, creationInfo.BaseTemplate, isSubSite);
            }

            return template;
        }

        private ProvisioningTemplate CleanupEntities(ProvisioningTemplate template, ProvisioningTemplate baseTemplate, bool isSubSite)
        {
            List<Guid> featuresToExclude = new List<Guid>();
            // Seems to be an feature left over on some older online sites...
            featuresToExclude.Add(Guid.Parse("d70044a4-9f71-4a3f-9998-e7238c11ce1a"));

            if (!isSubSite)
            {
                foreach (var feature in baseTemplate.Features.SiteFeatures)
                {
                    int index = template.Features.SiteFeatures.FindIndex(f => f.Id.Equals(feature.Id));

                    if (index > -1)
                    {
                        template.Features.SiteFeatures.RemoveAt(index);
                    }
                }

                foreach (var feature in featuresToExclude)
                {
                    int index = template.Features.SiteFeatures.FindIndex(f => f.Id.Equals(feature));

                    if (index > -1)
                    {
                        template.Features.SiteFeatures.RemoveAt(index);
                    }
                }

            }

            foreach (var feature in baseTemplate.Features.WebFeatures)
            {
                int index = template.Features.WebFeatures.FindIndex(f => f.Id.Equals(feature.Id));

                if (index > -1)
                {
                    template.Features.WebFeatures.RemoveAt(index);
                }
            }

            foreach (var feature in featuresToExclude)
            {
                int index = template.Features.WebFeatures.FindIndex(f => f.Id.Equals(feature));

                if (index > -1)
                {
                    template.Features.WebFeatures.RemoveAt(index);
                }
            }

            return template;
        }

    }
}
