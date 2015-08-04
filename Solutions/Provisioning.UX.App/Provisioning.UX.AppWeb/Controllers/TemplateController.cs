using OfficeDevPnP.Core.WebAPI;
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

namespace Provisioning.UX.AppWeb.Controllers
{
    /// <summary>
    /// Web API Class to work with Site Templates
    /// </summary>
    public class TemplateController : ApiController
    {
        #region Public Members
        [HttpPut]
        public void Register(WebAPIContext sharePointServiceContext)
        {
            WebAPIHelper.AddToCache(sharePointServiceContext);
        }
      
        /// <summary>
        /// Returns a list of available site templates that are available
        /// </summary>
        /// <returns></returns>
        [Route("api/provisioning/templates/getAvailableTemplates")]
        [WebAPIContextFilter]
        [HttpGet]
        public HttpResponseMessage GetSiteTemplates()
        {
            try
            {
                var _siteFactory = SiteTemplateFactory.GetInstance();
                var _tm = _siteFactory.GetManager();
                var _templates = _tm.GetAvailableTemplates();
                return Request.CreateResponse((HttpStatusCode)200, _templates);
            }
            catch (Exception _ex)
            {
                var _message = string.Format("There was an error processing the request. {0}", _ex.Message);
                Log.Error("TemplateController.GetSiteTemplates", "There was an error processing the request. Exception: {0}", _ex);
                HttpResponseMessage _response = Request.CreateResponse(HttpStatusCode.InternalServerError, _message);
                throw new HttpResponseException(_response); 
            }
        }
        #endregion
    }
}
