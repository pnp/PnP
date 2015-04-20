using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.Core.WebAPI;
using Provisioning.Common.Configuration;
using Provisioning.Common.Configuration.Template;
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
                _request.Success = true;
            }
            catch (Exception ex)
            {
                _request.ErrorMessage = ex.Message;
            }
            return _request;

        }


        #region Private Members 
        #endregion
    }
}
