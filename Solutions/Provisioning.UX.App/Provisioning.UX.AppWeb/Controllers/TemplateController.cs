using OfficeDevPnP.Core.WebAPI;
using Provisioning.Common.Data.SiteRequests;
using Provisioning.Common.Data.Templates;
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
        /// Returns a list of available site templates to create
        /// </summary>
        /// <returns></returns>
        [Route("api/provisioning/templates/getAvailableTemplates")]
        [WebAPIContextFilter]
        [HttpGet]
        public TemplateResultResponse GetSiteTemplates()
        {
            var _returnResponse = new TemplateResultResponse();
            _returnResponse.Success = false;

            try
            {
                var _siteFactory = SiteTemplateFactory.GetInstance();
                var _tm = _siteFactory.GetManager();
                _returnResponse.Templates = _tm.GetAvailableTemplates();
                _returnResponse.Success = true;
            }
            catch (Exception _ex)
            {
                _returnResponse.ErrorMessage = _ex.Message;
            }

            return _returnResponse;
        }
        #endregion
    }
}
