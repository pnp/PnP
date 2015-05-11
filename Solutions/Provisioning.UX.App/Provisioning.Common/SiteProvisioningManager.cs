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
using Provisioning.Common.Configuration;
using Provisioning.Common.Utilities;

namespace Provisioning.Common
{
    /// <summary>
    /// Implementation class that is used to create site collections
    /// </summary>
    public class SiteProvisioningManager
    {
        #region instance Members
        AbstractSiteProvisioningService _siteprovisioningService;
        const string CONNECTIONSTRING_KEY = "ConnectionString";
        const string CONTAINERSTRING_KEY = "Container";
        #endregion

        public SiteProvisioningManager(SiteRequestInformation siteRequest, Template template)
        {
            if (template.SharePointOnPremises)
            {
                _siteprovisioningService = new OnPremSiteProvisioningService();
            }
            else
            {
                _siteprovisioningService = new Office365SiteProvisioningService();
            }
        }
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="siteRequest"></param>
        /// <param name="template"></param>
        public void ProcessSiteRequest(SiteRequestInformation siteRequest, Template template)
        {
            _siteprovisioningService.Authentication = new AppOnlyAuthenticationTenant();
            _siteprovisioningService.Authentication.TenantAdminUrl = template.TenantAdminUrl;
            _siteprovisioningService.CreateSiteCollection(siteRequest, template);

        }
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="web"></param>
        public void ApplyProvisioningTemplates(ProvisioningTemplate provisioningTemplate, SiteRequestInformation siteRequest)
        {
            this._siteprovisioningService.Authentication = new AppOnlyAuthenticationSite();
            this._siteprovisioningService.Authentication.SiteUrl = siteRequest.Url;
            var _web = _siteprovisioningService.GetWebByUrl(siteRequest.Url);

            provisioningTemplate.Connector = this.GetProvisioningConnector();
            provisioningTemplate = new TemplateConversion().HandleProvisioningTemplate(provisioningTemplate, siteRequest);
            _web.ApplyProvisioningTemplate(provisioningTemplate);
        }

        /// <summary>
        /// Returns Connectors
        /// </summary>
        /// <returns></returns>
        private FileConnectorBase GetProvisioningConnector()
        {
            var _configManager = new ConfigManager();
            var _module = _configManager.GetModuleByName(ModuleKeys.PROVISIONINGCONNECTORS_KEY);
            var _managerTypeString = _module.ModuleType;

            try
            {
                var type = _managerTypeString.Split(',');
                var typeName = type[0];
                var assemblyName = type[1];
                var instance = (FileConnectorBase)Activator.CreateInstance(assemblyName, typeName).Unwrap();
                instance.AddParameter(CONNECTIONSTRING_KEY, _module.ConnectionString);
                instance.AddParameter(CONTAINERSTRING_KEY, string.Empty);
                return instance;
            }
            catch (Exception _ex)
            {
                Log.Error("Provisioning.Common.SiteProvisioningManager", 
                    "There was an error loading Provisioning Connector. Exception {0}", 
                    _ex);
              throw;
            }
        }
    }
}
