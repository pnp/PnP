using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.Core.WebAPI;
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
        public List<SiteTemplate> GetSiteTemplates()
        {
            var _returnResults = new List<SiteTemplate>();
           
            var _st = new SiteTemplate();
            _st.Title = "CUSTOM1";
            _st.Description = "My Description";
            _st.ImageUrl = "../images/template-icon.png";
            _st.DisplayOrder = 1;

            var _st1 = new SiteTemplate();
            _st1.Title = "Cust2";
            _st1.Description = "TEMPLATE 2";
            _st1.ImageUrl = "../images/template-icon.png";
            _st1.DisplayOrder = 2;

            _returnResults.Add(_st);
            _returnResults.Add(_st1);
            return _returnResults;
        }

        // POST api/<controller>
        [Route("api/provisioning/siterequest")]
        [WebAPIContextFilter]
        [HttpPost]
        public SiteRequest SaveSiteRequest([FromBody]string value)
        {
            var _request = new SiteRequest();
            _request.success = false;

            try
            {
                _request = JsonUtility.Deserialize<SiteRequest>(value);
                var t = value;
                _request.success = true;
            }
            catch (Exception ex)
            {
                _request.errorMessage = ex.Message;
            }
            return _request;

        }
    }
}
