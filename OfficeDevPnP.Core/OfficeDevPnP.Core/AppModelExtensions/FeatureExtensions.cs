using System;
using System.Linq;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Utilities;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// Class that deals with feature activation and deactivation
    /// </summary>
    public static partial class FeatureExtensions
    {
        /// <summary>
        /// Activates a site collection or site scoped feature
        /// </summary>
        /// <param name="web">Web to be processed - can be root web or sub web</param>
        /// <param name="featureID">ID of the feature to activate</param>
        /// <param name="sandboxed">Set to true if the feature is defined in a sandboxed solution</param>
        public static void ActivateFeature(this Web web, Guid featureID, bool sandboxed = false)
        {
            Log.Info(Constants.LOGGING_SOURCE, CoreResources.FeatureExtensions_ActivateWebFeature, featureID);
            web.ProcessFeature(featureID, true, sandboxed);
        }

        /// <summary>
        /// Activates a site collection or site scoped feature
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="featureID">ID of the feature to activate</param>
        /// <param name="sandboxed">Set to true if the feature is defined in a sandboxed solution</param>
        public static void ActivateFeature(this Site site, Guid featureID, bool sandboxed = false)
        {
            Log.Info(Constants.LOGGING_SOURCE, CoreResources.FeatureExtensions_ActivateWebFeature, featureID);
            site.ProcessFeature(featureID, true, sandboxed);
        }

        /// <summary>
        /// Deactivates a site collection or site scoped feature
        /// </summary>
        /// <param name="web">Web to be processed - can be root web or sub web</param>
        /// <param name="featureID">ID of the feature to deactivate</param>
        public static void DeactivateFeature(this Web web, Guid featureID)
        {
            Log.Info(Constants.LOGGING_SOURCE, CoreResources.FeatureExtensions_DeactivateWebFeature, featureID);
            web.ProcessFeature(featureID, false, false);
        }

        /// <summary>
        /// Deactivates a site collection or site scoped feature
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="featureID">ID of the feature to deactivate</param>
        public static void DeactivateFeature(this Site site, Guid featureID)
        {
            Log.Info(Constants.LOGGING_SOURCE, CoreResources.FeatureExtensions_DeactivateWebFeature, featureID);
            site.ProcessFeature(featureID, false, false);
        }

        /// <summary>
        /// Checks if a feature is active
        /// </summary>
        /// <param name="site">Site to operate against</param>
        /// <param name="featureID">ID of the feature to check</param>
        /// <returns>True if active, false otherwise</returns>
        public static bool IsFeatureActive(this Site site, Guid featureID)
        {
            return IsFeatureActiveInternal(site.Features, featureID);
        }

        /// <summary>
        /// Checks if a feature is active
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="featureID">ID of the feature to check</param>
        /// <returns>True if active, false otherwise</returns>
        public static bool IsFeatureActive(this Web web, Guid featureID)
        {
            return IsFeatureActiveInternal(web.Features, featureID);
        }

        /// <summary>
        /// Checks if a feature is active in the given FeatureCollection.
        /// </summary>
        /// <param name="features">FeatureCollection to check in</param>
        /// <param name="featureID">ID of the feature to check</param>
        /// <returns>True if active, false otherwise</returns>
        private static bool IsFeatureActiveInternal(FeatureCollection features, Guid featureID)
        {
            bool featureIsActive = false;

            features.Context.Load(features);
            features.Context.ExecuteQueryRetry();

            Feature iprFeature = features.GetById(featureID);
            features.Context.Load(iprFeature, f => f.DefinitionId);
            features.Context.ExecuteQueryRetry();

            if (iprFeature != null && iprFeature.IsPropertyAvailable("DefinitionId") && !iprFeature.ServerObjectIsNull.Value && iprFeature.DefinitionId.Equals(featureID))
            {
                featureIsActive = true;
            }

            return featureIsActive;
        }

        /// <summary>
        /// Activates or deactivates a site collection scoped feature
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="featureID">ID of the feature to activate/deactivate</param>
        /// <param name="activate">True to activate, false to deactivate the feature</param>
        /// <param name="sandboxed">Set to true if the feature is defined in a sandboxed solution</param>
        private static void ProcessFeature(this Site site, Guid featureID, bool activate, bool sandboxed)
        {
            ProcessFeatureInternal(site.Features, featureID, activate, sandboxed ? FeatureDefinitionScope.Site : FeatureDefinitionScope.Farm);
        }

        /// <summary>
        /// Activates or deactivates a web scoped feature
        /// </summary>
        /// <param name="web">Web to be processed - can be root web or sub web</param>
        /// <param name="featureID">ID of the feature to activate/deactivate</param>
        /// <param name="activate">True to activate, false to deactivate the feature</param>
        /// <param name="sandboxed">True to specify that the feature is defined in a sandboxed solution</param>
        private static void ProcessFeature(this Web web, Guid featureID, bool activate, bool sandboxed)
        {
            ProcessFeatureInternal(web.Features, featureID, activate, sandboxed ? FeatureDefinitionScope.Site : FeatureDefinitionScope.Farm);
        }

        /// <summary>
        /// Activates or deactivates a site collection or web scoped feature
        /// </summary>
        /// <param name="features">Feature Collection which contains the feature</param>
        /// <param name="featureID">ID of the feature to activate/deactivate</param>
        /// <param name="activate">True to activate, false to deactivate the feature</param>
        /// <param name="scope">Scope of the feature definition</param>
        private static void ProcessFeatureInternal(FeatureCollection features, Guid featureID, bool activate, FeatureDefinitionScope scope)
        {
            features.Context.Load(features);
            features.Context.ExecuteQueryRetry();

            // The original number of active features...use this to track if the feature activation went OK
            int oldCount = features.Count();

            if (activate)
            {
                // GetById does not seem to work for site scoped features...if (clientSiteFeatures.GetById(featureID) == null)

                // FeatureDefinitionScope defines how the features have been deployed. All OOB features are farm deployed
                features.Add(featureID, true, scope);
                features.Context.ExecuteQueryRetry();

                // retry logic needed to make this more bulletproof :-(
                features.Context.Load(features);
                features.Context.ExecuteQueryRetry();

                int tries = 0;
                int currentCount = features.Count();
                while (currentCount <= oldCount && tries < 5)
                {
                    tries++;
                    features.Add(featureID, true, scope);
                    features.Context.ExecuteQueryRetry();
                    features.Context.Load(features);
                    features.Context.ExecuteQueryRetry();
                    currentCount = features.Count();
                }
            }
            else
            {
                try
                {
                    features.Remove(featureID, false);
                    features.Context.ExecuteQueryRetry();
                }
                catch (Exception ex)
                {
                    Log.Error(Constants.LOGGING_SOURCE, CoreResources.FeatureExtensions_FeatureActivationProblem, featureID, ex.Message);
                }
            }
        }

    }
}
