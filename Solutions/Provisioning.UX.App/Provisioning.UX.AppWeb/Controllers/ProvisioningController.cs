using Newtonsoft.Json;
using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.Core.WebAPI;
using Provisioning.Common;
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
                _returnResults.Add(_st);
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
                _request.ErrorMessage = ex.Message;
            }
            return _request;

        }
        #endregion

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
