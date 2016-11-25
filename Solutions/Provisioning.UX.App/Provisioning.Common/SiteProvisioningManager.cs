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
using Provisioning.Common.Data.SiteRequests;
using System.Net;

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

        ///
        /// Checks if site exists or not.
        ///
        /// The URL of the remote site.
        /// True : If the file exits, False if file not exists
        private bool RemoteSiteExists(string url)
        {
            HttpWebResponse response;
            try
            {
                Uri urlCheck = new Uri(url);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlCheck);
                request.Timeout = 15000;
                                
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (Exception)
                {
                    return false; //could not connect to the internet (maybe) 
                }
            }
            catch
            {
                //Any exception will return false.
                return false;
            }
            return response.StatusCode == HttpStatusCode.Found;
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

            ReflectionManager rm = new ReflectionManager();

            var siteUrlProvider = rm.GetSiteUrlProvider("SiteUrlProvider");
            if(siteUrlProvider != null)
            {
                var newUrl = siteUrlProvider.GenerateSiteUrl(siteRequest, template);
                if (!String.IsNullOrEmpty(newUrl))
                {
                    Log.Info("SiteProvisioningManager.CreateSiteCollection", "Site {0} was renamed to {1}", siteRequest.Url, newUrl);

                    SiteRequestFactory.GetInstance().GetSiteRequestManager().UpdateRequestUrl(siteRequest.Url, newUrl);
                    siteRequest.Url = newUrl;

                }
                
            }



           // Check to see if the site already exists before attempting to create it
            bool siteExists = _siteprovisioningService.SiteExists(siteRequest.Url.ToString());

            if (!siteExists)
            {
                _siteprovisioningService.CreateSiteCollection(siteRequest, template);
                if (siteRequest.EnableExternalSharing)
                {
                    _siteprovisioningService.SetExternalSharing(siteRequest);
                }
            }
            else
            {                
                Log.Info("SiteProvisioningManager.CreateSiteCollection", "Site already exists. Moving on to next provisioning step");                
            }           
        }

        /// <summary>
        /// Member to create a sub site
        /// </summary>
        /// <param name="siteRequest">The SiteRequest</param>
        /// <param name="template">The Template</param>
        public Web CreateSubSite(SiteInformation siteRequest, Template template)
        {
            Web newWeb = null;

            _siteprovisioningService.Authentication = new AppOnlyAuthenticationTenant();
            _siteprovisioningService.Authentication.TenantAdminUrl = template.TenantAdminUrl;

            ReflectionManager rm = new ReflectionManager();

            var siteUrlProvider = rm.GetSiteUrlProvider("SiteUrlProvider");
            if (siteUrlProvider != null)
            {
                var newUrl = siteUrlProvider.GenerateSiteUrl(siteRequest, template);
                if (!String.IsNullOrEmpty(newUrl))
                {
                    Log.Info("SiteProvisioningManager.CreateSiteCollection", "Site {0} was renamed to {1}", siteRequest.Url, newUrl);

                    SiteRequestFactory.GetInstance().GetSiteRequestManager().UpdateRequestUrl(siteRequest.Url, newUrl);
                    siteRequest.Url = newUrl;

                }
            }

            // Check to see if the site already exists before attempting to create it
            bool siteExists = _siteprovisioningService.SubSiteExists(siteRequest.Url.ToString());

            if (!siteExists)
            {
                newWeb = _siteprovisioningService.CreateSubSite(siteRequest, template);
               
            }
            else
            {
                Log.Info("Provisioning.Common.Office365SiteProvisioningService.CreateSubSite", PCResources.SiteCreation_Creation_Starting, siteRequest.Url);
                Uri siteUri = new Uri(siteRequest.Url);                
                string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
                string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;

                using (var ctx = TokenHelper.GetClientContextWithAccessToken(siteRequest.Url, accessToken))
                {
                    newWeb = ctx.Web;
                }
            }

            return newWeb;
        }

        /// <summary>
        /// Member to apply the Provisioning Tempalte to a site
        /// </summary>
        /// <param name="web"></param>
        /// <exception cref="ProvisioningTemplateException">An Exception that occurs when applying the template to a site</exception>
        public void ApplyProvisioningTemplate(ProvisioningTemplate provisioningTemplate, SiteInformation siteRequest, Template template)
        {
            try
            {
                this._siteprovisioningService.Authentication = new AppOnlyAuthenticationSite();
                this._siteprovisioningService.Authentication.SiteUrl = siteRequest.Url;
                this._siteprovisioningService.SetSitePolicy(siteRequest.SitePolicy);
                var _web = _siteprovisioningService.GetWebByUrl(siteRequest.Url);
                provisioningTemplate.Connector = this.GetProvisioningConnector();                
                provisioningTemplate = new TemplateConversion().HandleProvisioningTemplate(provisioningTemplate, siteRequest, template);

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

        public void UpdateRequestAccessEmail(SiteInformation siteRequest)
        {
            Uri siteUri = new Uri(siteRequest.Url);
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
            string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;

            using (var clientContext = TokenHelper.GetClientContextWithAccessToken(siteRequest.Url, accessToken))
            {
                // Push notifications feature activation 
                // This needs to be here until another approach is found where it is not needed
                clientContext.Web.ActivateFeature(new Guid("41e1d4bfb1a247f7ab80d5d6cbba3092"));

                // Update Request Access Email                
                clientContext.Load(clientContext.Web, w => w.RequestAccessEmail);
                clientContext.ExecuteQuery();

                clientContext.Web.RequestAccessEmail = siteRequest.SiteOwner.Name;
                clientContext.Web.Update();
                clientContext.Load(clientContext.Web, w => w.RequestAccessEmail);
                clientContext.ExecuteQuery();
            }
        }
        public void UpdateSiteDescription(SiteInformation siteRequest)
        {
            Uri siteUri = new Uri(siteRequest.Url);
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
            string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, siteUri.Authority, realm).AccessToken;

            using (var clientContext = TokenHelper.GetClientContextWithAccessToken(siteRequest.Url, accessToken))
            {
                // Update Site Description                
                clientContext.Load(clientContext.Web, w => w.Description);
                clientContext.ExecuteQuery();

                clientContext.Web.Description = siteRequest.Description;
                clientContext.Web.Update();
                clientContext.Load(clientContext.Web, w => w.Description);
                clientContext.ExecuteQuery();

            }
        }
    }
}
