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

        /// <summary>
        /// Activates Site Features
        /// </summary>
        /// <param name="url">The site url</param>
        /// <param name="featureID">The OOB feature guid</param>
        void ActivateSiteFeature(string url, Guid featureID);

        /// <summary>
        /// Activates Web Features
        /// </summary>
        /// <param name="url">The site url</param>
        /// <param name="featureID">The OOB feature guid</param>
        void ActivateWebFeature(string url, Guid featureID);
        

        /// <summary>
        /// Deploys CustomActions to the Site Collection
        /// </summary>
        /// <param name="url"></param>
        /// <param name="customAction"></param>
        void DeployWebCustomAction(string url, CustomActionEntity customAction);

        /// <summary>
        /// Deploys Fields to the Site 
        /// </summary>
        /// <param name="url">The Site Url</param>
        /// <param name="fieldXML">Represents a field XML element of the field</param>
        void DeployFields(string url, string fieldXML);


        /// <summary>
        /// Deploys Content Types to a site
        /// </summary>
        /// <param name="url">Url of the site</param>
        /// <param name="contentTypeXML">Represents a content type xml element</param>
        void DeployContentType(string url, string contentTypeXML);

       
    }
}
