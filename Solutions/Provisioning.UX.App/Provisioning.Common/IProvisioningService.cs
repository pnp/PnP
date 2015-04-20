using Provisioning.Common.Configuration.Template;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common
{
    /// <summary>
    /// Interface that is implemented for Site Provisioning Services for both On-prem and Office 365 Sites
    /// </summary>
    public interface IProvisioningService
    {
        /// <summary>
        /// Creates a site collection.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        Guid? ProvisionSite(SiteRequestInformation properties);

        /// <summary>
        /// Member to apply the Site Policy to a site collection 
        /// <see cref="https://technet.microsoft.com/en-us/library/jj219569.aspx"/>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="policyName"></param>
        void ApplySitePolicy(string url, string policyName);

        /// <summary>
        /// Sets Administrators for the Site Collection
        /// </summary>
        /// <param name="properties"></param>
        void SetAdministrators(SiteRequestInformation properties);
        
        /// <summary>
        /// Sets the Description of the Site Collection
        /// </summary>
        /// <param name="properties"></param>
        void SetSiteDescription(SiteRequestInformation properties);
        
        /// <summary>
        /// Returns the Site Collection ID
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Guid? GetSiteGuidByUrl(string url);

       
    }
}
