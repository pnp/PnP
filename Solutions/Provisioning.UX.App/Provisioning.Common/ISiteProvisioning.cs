
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Provisioning.Common.Data.Templates;

namespace Provisioning.Common
{
    /// <summary>
    /// Interface that is implemented for Site Provisioning Services for both On-Premises and Office 365 Sites
    /// </summary>
    public interface ISiteProvisioning
    {
        /// <summary>
        /// Creates a Site Collection
        /// </summary>
        /// <param name="siteRequest">The Site Request to create</param>
        /// <param name="template">The Master Configuration Template</param>
        /// <returns><see cref="Web"/>The newly created Web</returns>
        void CreateSiteCollection(SiteInformation siteRequest, Template template);

        /// <summary>
        /// Checks to see if External Sharing Is enabled
        /// </summary>
        /// <returns></returns>
        bool IsTenantExternalSharingEnabled(string tenantUrl);

        /// <summary>
        /// Enables External Sharing on a site.
        /// </summary>
        void SetExternalSharing(SiteInformation siteInfo);

        /// <summary>
        /// Returns aa collection of Site Policies
        /// </summary>
        /// <returns></returns>
        List<SitePolicyEntity> GetAvailablePolicies();

        /// <summary>
        /// Sets the Site Policy
        /// </summary>
        /// <param name="policyName"></param>
        void SetSitePolicy(string policyName);

        /// <summary>
        /// Gets the Site Policy Applied to the Site
        /// </summary>
        /// <returns></returns>
        SitePolicyEntity GetAppliedSitePolicy();

        /// <summary>
        /// Returns the Site Collection ID
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Guid? GetSiteGuidByUrl(string url);

        /// <summary>
        /// Get Web by Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Web GetWebByUrl(string url);

       
    }
}
