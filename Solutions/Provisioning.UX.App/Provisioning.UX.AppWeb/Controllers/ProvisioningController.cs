using Newtonsoft.Json;
using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.Core.WebAPI;
using Provisioning.Common;
using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.Common.Data;
using Provisioning.Common.Data.SiteRequests;
using Provisioning.Common.Data.Templates;
using Provisioning.Common.Utilities;
using Provisioning.UX.AppWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Provisioning.UX.AppWeb.Controllers
{
    /// <summary>
    /// Class
    /// </summary>
    public class ProvisioningController : ApiController
    {
     
        #region Public Members
        [HttpPut]
        public void Register(WebAPIContext sharePointServiceContext)
        {
            WebAPIHelper.AddToCache(sharePointServiceContext);
        }
      
        /// <summary>
        /// Returns a list of available site policies
        /// </summary>
        /// <returns></returns>
        [Route("api/provisioning/availableSitePolicies")]
        [WebAPIContextFilter]
        [HttpGet]
        public List<SitePolicyResults> GetSitePolicies()
        {
            var _returnResults = new List<SitePolicyResults>();
            ConfigManager _manager = new ConfigManager();

            AbstractSiteProvisioningService _siteService = new Office365SiteProvisioningService();
            var _auth = new AppOnlyAuthenticationTenant();
            _auth.SiteUrl = _manager.GetAppSettingsKey("SPHost");

            var _sitePolicies = _siteService.GetAvailablePolicies();
            foreach(var _sitePolicyEntity in _sitePolicies)
            {
                var _policy = new SitePolicyResults();
                _policy.Key = _sitePolicyEntity.Name;
                _policy.Value = _sitePolicyEntity.Description;
                _returnResults.Add(_policy);
            }
            return _returnResults;
        }
        #endregion

        [Route("api/provisioning/externalSharingEnabled")]
        [WebAPIContextFilter]
        [HttpPost]
        public ExternalSharingRequest IsExternalSharingEnabled([FromBody]string value)
        {
            var _request = JsonConvert.DeserializeObject<ExternalSharingRequest>(value);
            _request.Success = false;

            try
            {
                AppOnlyAuthenticationTenant _auth = new AppOnlyAuthenticationTenant();
                _auth.TenantAdminUrl = _request.TenantAdminUrl;
                var _service = new Office365SiteProvisioningService();
                _service.Authentication = _auth;
                _request.ExternalSharingEnabled = _service.IsTenantExternalSharingEnabled(_request.TenantAdminUrl);
                _request.Success = true;
                return _request;
            }
            catch(Exception _ex)
            {
               _request.ErrorMessage = _ex.Message;
                OfficeDevPnP.Core.Diagnostics.Log.Error("ProvisioningController.IsExternalSharingEnabled", 
                   "There was an error processing the request. Exception: {0}", 
                   _ex);
               return _request;
            }
    
        }

        [Route("api/provisioning/isSiteUrlProviderUsed")]
        [WebAPIContextFilter]
        [HttpPost]
        public SiteUrlCheckRequest IsSiteUrlProviderUsed([FromBody]string value)
        {
            var _request = JsonConvert.DeserializeObject<SiteUrlCheckRequest>(value);

            ReflectionManager rm = new ReflectionManager();

            var siteUrlProvider = rm.GetSiteUrlProvider("SiteUrlProvider");
            if (siteUrlProvider != null)
            {
                _request.UsesCustomProvider = false;
            }
            else
            {
                _request.UsesCustomProvider = true;
            }

            return _request;
        }




    }
}
