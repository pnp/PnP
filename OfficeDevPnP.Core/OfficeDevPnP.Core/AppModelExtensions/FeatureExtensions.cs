using Microsoft.SharePoint.Client;
using OfficeAMS.Core;
using OfficeAMS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class that deals with feature activation and deactivation
    /// </summary>
    public static class FeatureExtensions
    {
        const string MSG_PROBLEM_REMOVING = "Problem removing feature [{0}].";

        /// <summary>
        /// Activates a site collection or site scoped feature
        /// </summary>
        /// <param name="web">Web to be processed - can be root web or sub web</param>
        /// <param name="featureID">ID of the feature to activate</param>
        public static void ActivateFeature(this Web web, Guid featureID)
        {
            web.ProcessFeature(featureID, true);
        }


        /// <summary>
        /// Activates a site collection or site scoped feature
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="featureID">ID of the feature to activate</param>
        public static void ActivateFeature(this Site site, Guid featureID)
        {
            site.ProcessFeature(featureID, true);
        }

        /// <summary>
        /// Deactivates a site collection or site scoped feature
        /// </summary>
        /// <param name="web">Web to be processed - can be root web or sub web</param>
        /// <param name="featureID">ID of the feature to deactivate</param>
        public static void DeactivateFeature(this Web web, Guid featureID)
        {
            web.ProcessFeature(featureID, false);
        }

        /// <summary>
        /// Deactivates a site collection or site scoped feature
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="featureID">ID of the feature to deactivate</param>
        public static void DeactivateFeature(this Site site, Guid featureID)
        {
            site.ProcessFeature(featureID, false);
        }

        /// <summary>
        /// Activates or deactivates a site collection or site scoped feature
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="featureID">ID of the feature to activate/deactivate</param>
        /// <param name="activate">True to activate, false to deactivate the feature</param>
        private static void ProcessFeature(this Site site, Guid featureID, bool activate)
        {
            FeatureCollection clientSiteFeatures = site.Features;

            site.Context.Load(clientSiteFeatures);
            site.Context.ExecuteQuery();

            // The original number of active features...use this to track if the feature activation went OK
            int oldCount = clientSiteFeatures.Count();

            if (activate)
            {
                // GetById does not seem to work for site scoped features...if (clientSiteFeatures.GetById(featureID) == null)

                // FeatureDefinitionScope defines how the features have been deployed. All OOB features are farm deployed
                clientSiteFeatures.Add(featureID, true, FeatureDefinitionScope.Farm);
                site.Context.ExecuteQuery();

                // retry logic needed to make this more bulletproof :-(
                site.Context.Load(clientSiteFeatures);
                site.Context.ExecuteQuery();

                int tries = 0;
                int currentCount = clientSiteFeatures.Count();
                while (currentCount <= oldCount && tries < 5)
                {
                    tries++;
                    clientSiteFeatures.Add(featureID, true, FeatureDefinitionScope.Farm);
                    site.Context.ExecuteQuery();
                    site.Context.Load(clientSiteFeatures);
                    site.Context.ExecuteQuery();
                    currentCount = clientSiteFeatures.Count();
                }
            }
            else
            {
                try
                {
                    clientSiteFeatures.Remove(featureID, false);
                    site.Context.ExecuteQuery();
                }
                catch (Exception ex) {
                    LoggingUtility.LogError(string.Format(MSG_PROBLEM_REMOVING, featureID), ex, EventCategory.Features);
                }
            }
        }
        
        /// <summary>
        /// Activates or deactivates a site collection or site scoped feature
        /// </summary>
        /// <param name="web">Web to be processed - can be root web or sub web</param>
        /// <param name="featureID">ID of the feature to activate/deactivate</param>
        /// <param name="activate">True to activate, false to deactivate the feature</param>
        private static void ProcessFeature(this Web web, Guid featureID, bool activate)
        {
            FeatureCollection clientSiteFeatures = web.Features;

            web.Context.Load(clientSiteFeatures);
            web.Context.ExecuteQuery();

            // The original number of active features...use this to track if the feature activation went OK
            int oldCount = clientSiteFeatures.Count();

            if (activate)
            {
                // GetById does not seem to work for site scoped features...if (clientSiteFeatures.GetById(featureID) == null)

                // FeatureDefinitionScope defines how the features have been deployed. All OOB features are farm deployed
                clientSiteFeatures.Add(featureID, true, FeatureDefinitionScope.Farm);
                web.Context.ExecuteQuery();

                // retry logic needed to make this more bulletproof :-(
                web.Context.Load(clientSiteFeatures);
                web.Context.ExecuteQuery();

                int tries = 0;
                int currentCount = clientSiteFeatures.Count();
                while (currentCount <= oldCount && tries < 5)
                {
                    tries++;
                    clientSiteFeatures.Add(featureID, true, FeatureDefinitionScope.Farm);
                    web.Context.ExecuteQuery();
                    web.Context.Load(clientSiteFeatures);
                    web.Context.ExecuteQuery();
                    currentCount = clientSiteFeatures.Count();
                }
            }
            else
            {
                try
                {
                    clientSiteFeatures.Remove(featureID, false);
                    web.Context.ExecuteQuery();
                }
                catch(Exception ex)
                {
                    LoggingUtility.LogError(string.Format(MSG_PROBLEM_REMOVING, featureID), ex, EventCategory.Features);
                }
            }
        }

    }
}
