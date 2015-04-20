using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.Core.WebAPI;
using Provisioning.Common;
using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Template;
using Provisioning.Common.Data;
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
    [EnableCors(origins: "*", headers: "*", methods: "*")] 
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
        #endregion

        [Route("api/provisioning/availabletemplates")]
        [WebAPIContextFilter]
        [HttpGet]
        public List<SiteTemplateResults> GetSiteTemplates()
        {
            var _returnResults = new List<SiteTemplateResults>();

            var _configFactory = ConfigurationFactory.GetInstance();
            var _tf = _configFactory.GetTemplateFactory();
            TemplateManager _tm = _tf.GetTemplateManager();
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

        // POST api/<controller>
        [Route("api/provisioning/siterequest")]
        [WebAPIContextFilter]
        [HttpPost]
        public SiteRequest SaveSiteRequest([FromBody]string value)
        {
            var _request = new SiteRequest();
            _request.Success = false;

            try
            {
                _request = JsonUtility.Deserialize<SiteRequest>(value);
                var t = value;
                this.SaveSiteRequestToRepository(_request);
                _request.Success = true;
            }
            catch (Exception ex)
            {
                _request.ErrorMessage = ex.Message;
            }
            return _request;

        }

        #region Private Members 

        private void SaveSiteRequestToRepository(SiteRequest siteRequest)
        {
            try
            {
                var _owner = new SharePointUser()
                {
                    Email = siteRequest.PrimaryOwner
                };

                List<SharePointUser> _additionalAdmins = new List<SharePointUser>();

                foreach(var secondaryOwner in siteRequest.SecondaryOwners)
                {
                    var _sharePointUser = new SharePointUser();
                    _sharePointUser.Email = secondaryOwner;
                    _additionalAdmins.Add(_sharePointUser);
                }

                var _newRequest = new SiteRequestInformation();
                _newRequest.Title = siteRequest.Title;
                _newRequest.Description = siteRequest.Description;
                _newRequest.Url = string.Format("{0}{1}", siteRequest.HostPath, siteRequest.Url);
                _newRequest.Template = siteRequest.Template;
                _newRequest.SitePolicy = siteRequest.SitePolicy;
                _newRequest.SiteOwner = _owner;
                _newRequest.AdditionalAdministrators = _additionalAdmins;
                _newRequest.SharePointOnPremises = siteRequest.SharePointOnPremises;
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
