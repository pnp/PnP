using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Core
{
    public static class SPOFeatures
    {
        /// <summary>
        /// Activates a feature on the current web. The Feature Definition is assumed to be site scoped.
        /// </summary>
        /// <param name="featureId"></param>
        /// <param name="force"></param>
        public static Feature ActivateFeature(Guid featureId, bool force, ClientContext clientContext)
        {
            return ActivateFeature(featureId, force, FeatureScope.Web, clientContext);
        }

        /// <summary>
        /// Activates a feature on the current web
        /// </summary>
        /// <param name="featureId"></param>
        /// <param name="force"></param>
        /// <param name="scope"></param>
        public static Feature ActivateFeature(Guid featureId, bool force, FeatureScope featureScope, ClientContext clientContext)
        {
            Feature f = null;
            if (featureScope == FeatureScope.Site)
            {
                Site site = clientContext.Site;
                f = site.Features.Add(featureId, force, FeatureDefinitionScope.None);
            }
            else
            {
                Web web = clientContext.Web;
                f = web.Features.Add(featureId, force, FeatureDefinitionScope.None);
            }
            clientContext.Load(f);
            clientContext.ExecuteQuery();

            return f;
        }

        /// <summary>
        /// Deactivates a feature in the current scope;
        /// </summary>
        /// <param name="featureId"></param>
        /// <param name="force"></param>
        /// <param name="featureScope"></param>
        /// <param name="clientContext"></param>
        public static void DeactivateFeature(Guid featureId, bool force, FeatureScope featureScope, ClientContext clientContext)
        {
            if (featureScope == FeatureScope.Site)
            {
                Site site = clientContext.Site;
                site.Features.Remove(featureId, force);
            }
            else
            {
                Web web = clientContext.Web;
                web.Features.Remove(featureId, force);
            }

            clientContext.ExecuteQuery();
        }

        /// <summary>
        /// Deactivates a feature in the current web
        /// </summary>
        /// <param name="featureId"></param>
        /// <param name="force"></param>
        /// <param name="clientContext"></param>
        public static void DeactivateFeature(Guid featureId, bool force, ClientContext clientContext)
        {
            DeactivateFeature(featureId, force, FeatureScope.Web, clientContext);
        }
            
        public enum FeatureScope
        {
            Site = 0,
            Web = 1
        }

        public static List<Feature> GetFeatures(FeatureScope scope, ClientContext clientContext)
        {
            List<Feature> features = new List<Feature>();
            FeatureCollection featureCollection = null;
            if (scope == FeatureScope.Site)
            {
                featureCollection = clientContext.Site.Features;
            }
            else
            {
                featureCollection = clientContext.Web.Features;
            }

            clientContext.Load(featureCollection);
            clientContext.ExecuteQuery();
            foreach (var f in featureCollection)
            {
                features.Add(f);
            }
            return features;
        }
    }
}
