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
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;

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

        public SiteProvisioningManager(SiteInformation siteRequest, Template template)
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
        /// Member to create a site collection
        /// </summary>
        /// <param name="siteRequest">The SiteRequest</param>
        /// <param name="template">The Template</param>
        public void CreateSiteCollection(SiteInformation siteRequest, Template template)
        {
            _siteprovisioningService.Authentication = new AppOnlyAuthenticationTenant();
            _siteprovisioningService.Authentication.TenantAdminUrl = template.TenantAdminUrl;
     
            _siteprovisioningService.CreateSiteCollection(siteRequest, template);
            if(siteRequest.EnableExternalSharing)
            {
                _siteprovisioningService.SetExternalSharing(siteRequest);
            }
           
        }
        /// <summary>
        /// Member to apply the Provisioning Tempalte to a site
        /// </summary>
        /// <param name="web"></param>
        /// <exception cref="ProvisioningTemplateException">An Exception that occurs when applying the template to a site</exception>
        public void ApplyProvisioningTemplate(ProvisioningTemplate provisioningTemplate, SiteInformation siteRequest)
        {
            try
            {
                this._siteprovisioningService.Authentication = new AppOnlyAuthenticationSite();
                this._siteprovisioningService.Authentication.SiteUrl = siteRequest.Url;
                var _web = _siteprovisioningService.GetWebByUrl(siteRequest.Url);
                provisioningTemplate.Connector = this.GetProvisioningConnector();
                provisioningTemplate = new TemplateConversion().HandleProvisioningTemplate(provisioningTemplate, siteRequest);

                ProvisioningTemplateApplyingInformation _pta = new ProvisioningTemplateApplyingInformation();
                _pta.ProgressDelegate = (message, step, total) =>
                {
                    Log.Info("SiteProvisioningManager.ApplyProvisioningTemplate", "Applying Provisioning template - Step {0}/{1} : {2} ", step, total, message);
                }; 
                _web.ApplyProvisioningTemplate(provisioningTemplate);
            }
            catch(Exception _ex)
            {
                var _message =string.Format("Error Occured when applying the template: {0} to site: {1}", _ex.Message, siteRequest.Url);
                throw new ProvisioningTemplateException(_message, _ex);
            }
        }
        /// <summary>
        /// Returns Connectors
        /// </summary>
        /// <returns></returns>
        private FileConnectorBase GetProvisioningConnector()
        {
            ReflectionManager _helper = new ReflectionManager();
            FileConnectorBase _connectorInstance =  _helper.GetProvisioningConnector(ModuleKeys.PROVISIONINGCONNECTORS_KEY);
            return _connectorInstance;
        }
    }
}
