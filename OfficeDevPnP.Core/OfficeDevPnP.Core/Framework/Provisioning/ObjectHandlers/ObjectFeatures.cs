using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;

namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectFeatures : ObjectHandlerBase
    {
        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            var webFeatures = template.Features.WebFeatures;
            var siteFeatures = template.Features.SiteFeatures;
            ProvisionWebFeaturesImplementation(web, webFeatures);
            ProvisionSiteFeaturesImplementation(web, siteFeatures);

        }

        private static void ProvisionWebFeaturesImplementation(Web container, List<WebFeature> webFeatures)
        {
            if (webFeatures != null)
            {
                foreach (var feature in webFeatures)
                {
                    var featureId = Guid.Empty;

                    if (Guid.TryParse(feature.ID, out featureId))
                    {
                        if (!feature.Deactivate)
                        {
                            if (!container.IsFeatureActive(featureId))
                            {
                                container.ActivateFeature(featureId);
                            }
                        }
                        else
                        {
                            if (container.IsFeatureActive(featureId))
                            {
                                container.DeactivateFeature(featureId);
                            }
                        }
                    }
                }
            }
        }

        private static void ProvisionSiteFeaturesImplementation(Web container, List<SiteFeature> features)
        {
            if (features != null)
            {
                foreach (var feature in features)
                {
                    var featureId = Guid.Empty;

                    if (Guid.TryParse(feature.ID, out featureId))
                    {
                        if (!feature.Deactivate)
                        {
                            if (!container.IsFeatureActive(featureId))
                            {
                                container.ActivateFeature(featureId);
                            }
                        }
                        else
                        {
                            if (container.IsFeatureActive(featureId))
                            {
                                container.DeactivateFeature(featureId);
                            }
                        }
                    }
                }
            }
        }

        public override Model.ProvisioningTemplate CreateEntities(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            var context = web.Context as ClientContext;
            var webFeatures = web.Features;
            var siteFeatures = context.Site.Features;

            context.Load(webFeatures, fs => fs.Include(f => f.DefinitionId));
            context.Load(siteFeatures, fs => fs.Include(f => f.DefinitionId));
            context.ExecuteQueryRetry();

            var features = new Features();
            foreach (var feature in webFeatures)
            {
                features.WebFeatures.Add(new WebFeature() { Deactivate = false, ID = feature.DefinitionId.ToString() });
            }
            foreach (var feature in siteFeatures)
            {
                features.SiteFeatures.Add(new SiteFeature() { Deactivate = false, ID = feature.DefinitionId.ToString() });
            }

            template.Features = features;

            return template;
        }
    }
}
