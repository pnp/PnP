using Newtonsoft.Json;
using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.Core.WebAPI;
using Provisioning.Common;
using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.Common.Data;
using Provisioning.Common.Data.SiteRequests;
using Provisioning.Common.Data.Templates;
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
    public class ProvisioningController : ApiController
    {
        #region Instance Members
     
        #endregion

        #region Public Members
        [HttpPut]
        public void Register(WebAPIContext sharePointServiceContext)
        {
            WebAPIHelper.AddToCache(sharePointServiceContext);
        }
        
        /// <summary>
        /// Returns a list of available site templates to create
        /// </summary>
        /// <returns></returns>
        [Route("api/provisioning/availabletemplates")]
        [WebAPIContextFilter]
        [HttpGet]
        public List<SiteTemplateResults> GetSiteTemplates()
        {
            var _returnResults = new List<SiteTemplateResults>();
            var _siteFactory = SiteTemplateFactory.GetInstance();
            var _tm = _siteFactory.GetManager();
            var _templates = _tm.GetAvailableTemplates();

            foreach(var _t in _templates)
            {
                var _st = new SiteTemplateResults();
                _st.Title = _t.Title;
                _st.Description = _t.Description;
                _st.ImageUrl = _t.ImageUrl;
                _st.HostPath = _t.HostPath;
                _st.SharePointOnPremises = _t.SharePointOnPremises;
                _st.TenantAdminUrl = _t.TenantAdminUrl;
                _returnResults.Add(_st);
            }
            return _returnResults;
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
      
        /// <summary>
        /// Saves a site request to the Data Repository
        /// POST api/<controller>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Route("api/provisioning/siterequest")]
        [WebAPIContextFilter]
        [HttpPost]
        public SiteRequest SaveSiteRequest([FromBody]string value)
        {
            var _request = new SiteRequest();
            _request.Success = false;

            try
            {
                _request = JsonConvert.DeserializeObject<SiteRequest>(value);
                this.SaveSiteRequestToRepository(_request);
                _request.Success = true;
            }
            catch (Exception ex)
            {
                Log.Error("Provisioning.UX.AppWeb.Controllers.ProvisioningController", 
                    "There was an error saving the Site Request. Error Message {0} Error Stack {1}",
                    ex.Message,
                    ex);
                _request.ErrorMessage = ex.Message;
            }
            return _request;

        }
        #endregion

        [Route("api/provisioning/externalSharingEnabled")]
        [WebAPIContextFilter]
        [HttpPost]
        public ExternalSharingRequest IsExternalSharingEnabled([FromBody]string value)
        {
            var _request = JsonConvert.DeserializeObject<ExternalSharingRequest>(value);
            _request.Success = true;

            AppOnlyAuthenticationTenant _auth = new AppOnlyAuthenticationTenant();
            _auth.TenantAdminUrl = _request.TenantAdminUrl;
            var _service = new Office365SiteProvisioningService();
            _service.Authentication = _auth;
            _request.ExternalSharingEnabled = _service.IsTenantExternalSharingEnabled(_request.TenantAdminUrl);
            return _request;
        }

        #region Private Members
        /// <summary>
        /// Save the Site Request to the Data Repository
        /// </summary>
        /// <param name="siteRequest"></param>
        private void SaveSiteRequestToRepository(SiteRequest siteRequest)
        {
            try
            {
                var _newRequest = ObjectMapper.ToSiteRequestInformation(siteRequest);
                ///Save the Site Request
                ISiteRequestFactory _srf = SiteRequestFactory.GetInstance();
                var _manager = _srf.GetSiteRequestManager();
                _manager.CreateNewSiteRequest(_newRequest);
            }
            catch (Exception _ex)
            {
                throw;
            }
          
        }
        #endregion
    }
}
