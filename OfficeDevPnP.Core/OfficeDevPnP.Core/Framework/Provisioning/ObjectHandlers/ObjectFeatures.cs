using System.Collections.Generic;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using Feature = OfficeDevPnP.Core.Framework.Provisioning.Model.Feature;
using System;
using System.Linq;
using OfficeDevPnP.Core.Utilities;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    internal class ObjectFeatures : ObjectHandlerBase
    {
        public override string Name
        {
            get { return "Features"; }
        }

        public override void ProvisionObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateApplyingInformation applyingInformation)
        {
            Log.Info(Constants.LOGGING_SOURCE_FRAMEWORK_PROVISIONING, CoreResources.Provisioning_ObjectHandlers_Features);

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
            }
        }

        public override ProvisioningTemplate ExtractObjects(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
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
            if (!isSubSite)
            {
                var cleanSiteFeatures = GetCleanFeatures(template.Features.SiteFeatures, baseTemplate.Features.SiteFeatures);
                template.Features.SiteFeatures.Clear();
                cleanSiteFeatures.ForEach(x => template.Features.SiteFeatures.Add(x));
            }

            var cleanWebFeatures = GetCleanFeatures(template.Features.WebFeatures, baseTemplate.Features.WebFeatures);
            template.Features.WebFeatures.Clear();
            cleanWebFeatures.ForEach(x => template.Features.WebFeatures.Add(x));

            return template;
        }

        private List<Feature> GetCleanFeatures(List<Feature> templateFeatures, List<Feature> baseFeatures)
        {
            // Seems to be an feature left over on some older online sites...
            var featuresToExclude = new List<Guid> { Guid.Parse("d70044a4-9f71-4a3f-9998-e7238c11ce1a") };

            var activatedFeatures = templateFeatures
                    .Where(x => !x.Deactivate && !featuresToExclude.Any(y => y == x.Id))
                    .ToList();

            var duplicatedFeatures = activatedFeatures
                .Where(x => baseFeatures.Any(y => y.Id == x.Id && !y.Deactivate));

            var deactivatedFeatures = baseFeatures
                .Where(x => !templateFeatures.Any(y => y.Id == x.Id && !y.Deactivate))
                .Select(x => new Feature {Id = x.Id, Deactivate = true});

            return activatedFeatures
                .Concat(deactivatedFeatures)
                .Except(duplicatedFeatures)
                .ToList();
        }


        public override bool WillProvision(Web web, ProvisioningTemplate template)
        {
            if (!_willProvision.HasValue)
            {
                _willProvision = template.Features.SiteFeatures.Any() || template.Features.WebFeatures.Any();
            }
            return _willProvision.Value;
        }

        public override bool WillExtract(Web web, ProvisioningTemplate template, ProvisioningTemplateCreationInformation creationInfo)
        {
            if (!_willExtract.HasValue)
            {
                _willExtract = true;
            }
            return _willExtract.Value;
        }
    }
}
