using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;


namespace OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers
{
    public class ObjectFeatures : ObjectHandlerBase
    {
        public override void ProvisionObjects(Microsoft.SharePoint.Client.Web web, Model.ProvisioningTemplate template)
        {
            var context = web.Context as ClientContext;

            var webFeatures = template.Features.WebFeatures;
            var siteFeatures = template.Features.SiteFeatures;
            ProvisionFeaturesImplementation(web, webFeatures);
            ProvisionFeaturesImplementation(context.Site, siteFeatures);
        }

        private static void ProvisionFeaturesImplementation(object parent, List<OfficeDevPnP.Core.Framework.Provisioning.Model.Feature> webFeatures)
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
                features.WebFeatures.Add(new Model.Feature() { Deactivate = false, ID = feature.DefinitionId });
            }
            foreach (var feature in siteFeatures)
            {
                features.SiteFeatures.Add(new Model.Feature() { Deactivate = false, ID = feature.DefinitionId });
            }

            template.Features = features;

            return template;
        }
    }
}
