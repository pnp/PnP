using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Hybrid.Simple.Common
{
    public class SiteProvisioningManager
    {
        /// <summary>
        /// Main business logic to drive provisioning
        /// </summary>
        /// <param name="message">Request details</param>
        /// <param name="serviceBusNamespace">Service bus namespace, used if on+p</param>
        /// <param name="serviceBusSecret">Service bus secret</param>
        /// <returns></returns>
        public bool ProcessOnPremSiteRequest(SiteCollectionRequest request, string serviceBusNamespace, string serviceBusSecret)
        {

            return true;
        }

        /// <summary>
        /// Main business logic to drive provisioning
        /// </summary>
        /// <param name="message">Request details</param>
        /// <param name="serviceBusNamespace">Service bus namespace, used if on+p</param>
        /// <param name="serviceBusSecret">Service bus secret</param>
        /// <returns></returns>
        public bool ProcessCloudSiteRequest(SiteCollectionRequest request)
        {

            return false;
        }
    }
}
