using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using Feature = OfficeDevPnP.Core.Framework.Provisioning.Model.Feature;
using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectFeatures : ObjectHandlerBase
    {
        public override void ProvisionObjects(Web web, ProvisioningTemplate template)
        {
            var context = web.Context as ClientContext;
            
            // if this is a sub site then we're not enabling the site collection scoped features
            if (!web.IsSubSite())
            {
                var siteFeatures = template.Features.SiteFeatures;
                ProvisionFeaturesImplementation(context.Site, siteFeatures);
            }

            var webFeatures = template.Features.WebFeatures;
            ProvisionFeaturesImplementation(web, webFeatures);
        }

        private static void ProvisionFeaturesImplementation(object parent, List<Feature> webFeatures)
        {
            Web web = null;
            Site site = null;
            if (parent is Site)
            {
                site = parent as Site;
            }
            else
            {
                web = parent as Web;
            }

            if (webFeatures != null)
            {
                foreach (var feature in webFeatures)
                {
                    if (!feature.Deactivate)
                    {
                        if (site != null)
                        {
                            if (!site.IsFeatureActive(feature.ID))
                            {
                                site.ActivateFeature(feature.ID);
                            }
                        }
                        else
                        {
                            if (!web.IsFeatureActive(feature.ID))
                            {
                                web.ActivateFeature(feature.ID);
                            }
                        }

                    }
                    else
                    {
                        if (site != null)
                        {
                            if (site.IsFeatureActive(feature.ID))
                            {
                                site.DeactivateFeature(feature.ID);

                            }
                        }
                        else
                        {
                            if (web.IsFeatureActive(feature.ID))
                            {
                                web.DeactivateFeature(feature.ID);
                            }
                        }
                    }

                }
            }
        }


        public override ProvisioningTemplate CreateEntities(Web web, ProvisioningTemplate template, ProvisioningTemplate baseTemplate)
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
                features.WebFeatures.Add(new Feature() { Deactivate = false, ID = feature.DefinitionId });
            }

            // if this is a sub site then we're not creating  site collection scoped feature entities
            if (!isSubSite)
            {
                foreach (var feature in siteFeatures)
                {
                    features.SiteFeatures.Add(new Feature() { Deactivate = false, ID = feature.DefinitionId });
                }
            }

            template.Features = features;

            // If a base template is specified then use that one to "cleanup" the generated template model
            if (baseTemplate != null)
            {
                template = CleanupEntities(template, baseTemplate, isSubSite);
            }

            return template;
        }

        private ProvisioningTemplate CleanupEntities(ProvisioningTemplate template, ProvisioningTemplate baseTemplate, bool isSubSite)
        {
            List<Guid> featuresToExclude = new List<Guid>();
            featuresToExclude.Add(Guid.Parse("d70044a4-9f71-4a3f-9998-e7238c11ce1a"));

            if (!isSubSite)
            {
                foreach (var feature in baseTemplate.Features.SiteFeatures)
                {
                    int index = template.Features.SiteFeatures.FindIndex(f => f.ID.Equals(feature.ID));

                    if (index > -1)
                    {
                        template.Features.SiteFeatures.RemoveAt(index);
                    }
                }

                foreach(var feature in featuresToExclude)
                {
                    int index = template.Features.SiteFeatures.FindIndex(f => f.ID.Equals(feature));

                    if (index > -1)
                    {
                        template.Features.SiteFeatures.RemoveAt(index);
                    }
                }

            }

            foreach (var feature in baseTemplate.Features.WebFeatures)
            {
                int index = template.Features.WebFeatures.FindIndex(f => f.ID.Equals(feature.ID));

                if (index > -1)
                {
                    template.Features.WebFeatures.RemoveAt(index);
                }
            }

            foreach (var feature in featuresToExclude)
            {
                int index = template.Features.WebFeatures.FindIndex(f => f.ID.Equals(feature));

                if (index > -1)
                {
                    template.Features.WebFeatures.RemoveAt(index);
                }
            }

            return template;
        }

    }
}
