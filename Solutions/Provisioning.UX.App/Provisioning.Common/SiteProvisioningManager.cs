using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using Provisioning.Common.Authentication;
using Provisioning.Common.Data.Templates;

namespace Provisioning.Common
{
    /// <summary>
    /// Implementation class that is used to create site collections
    /// </summary>
    public class SiteProvisioningManager
    {
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="siteRequest"></param>
        /// <param name="template"></param>
        public Web ProcessSiteRequest(SiteRequestInformation siteRequest, Template template)
        {
            AbstractSiteProvisioningService _siteprovisioningService;

            if(template.SharePointOnPremises)
            {
                _siteprovisioningService = new OnPremSiteProvisioningService();
            }
            else
            {
                _siteprovisioningService = new Office365SiteProvisioningService();
            }

            _siteprovisioningService.Authentication = new AppOnlyAuthenticationTenant();
            _siteprovisioningService.Authentication.TenantAdminUrl = template.TenantAdminUrl;
            var _web = _siteprovisioningService.CreateSiteCollection(siteRequest, template);

            return _web;
       }

        public Web GetWeb(SiteRequestInformation siteRequest, Template template)
        {
            var t = new Office365SiteProvisioningService();
            t.Authentication = new AppOnlyAuthenticationTenant();
            var web = t.GeWebByUrl(siteRequest.Url);
            return web;

        }
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="web"></param>
        public void ApplyProvisioningTemplates(Web web, ProvisioningTemplate provisioningTemplate)
        {
          //  var connector;
            provisioningTemplate.Connector = new FileSystemConnector("Resources/SiteTemplates/ProvisioningTemplates/", string.Empty);
            web.ApplyProvisioningTemplate(provisioningTemplate);
        }


    }
}
