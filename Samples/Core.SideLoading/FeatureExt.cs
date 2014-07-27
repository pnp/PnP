//*********************************************************
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//*********************************************************

using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SideLoading
{
    public static class FeatureExtensions
    {
        /// <summary>
        /// Activates or deactivates a site collection or site scoped feature
        /// </summary>
        /// <param name="site">Site to be processed</param>
        /// <param name="featureID">ID of the feature to activate/deactivate</param>
        /// <param name="activate">True to activate, false to deactivate the feature</param>
        public static void ProcessFeature(this Site site, Guid featureID, bool activate)
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
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
