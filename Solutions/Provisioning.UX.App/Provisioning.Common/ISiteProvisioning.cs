
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
        Web CreateSiteCollection(SiteRequestInformation siteRequest, Template template);

        /// <summary>
        /// Returns the Site Collection ID
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Guid? GetSiteGuidByUrl(string url);

        Web GetWebByUrl(string url);

       
    }
}
