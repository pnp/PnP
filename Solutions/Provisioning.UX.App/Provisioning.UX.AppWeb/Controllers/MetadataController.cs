using Newtonsoft.Json;
using OfficeDevPnP.Core.WebAPI;
using Provisioning.Common;
using Provisioning.Common.Data.Metadata;
using Provisioning.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Provisioning.UX.AppWeb.Controllers
{
    [Authorize]
    public class MetadataController : ApiController
    {
        [HttpPut]
        public void Register(WebAPIContext sharePointServiceContext)
        {
            WebAPIHelper.AddToCache(sharePointServiceContext);
        }

        /// <summary>
        /// Returns a list of available site templates that are available
        /// </summary>
        /// <returns></returns>
        [Route("api/provisioning/sitepolicies/getSitePolicies")]
        [HttpGet]
        public HttpResponseMessage GetSitePolices()
        {  
            try
            {
                IMetadataFactory _factory = MetadataFactory.GetInstance();
                IMetadataManager _manager = _factory.GetManager();
                var _siteClassifications = _manager.GetAvailableSiteClassifications();
                return Request.CreateResponse(HttpStatusCode.OK, _siteClassifications);
            }
            catch (Exception _ex)
            {
                var _message = string.Format("There was an error processing the request. {0}", _ex.Message);
                Log.Error("MetadataController.GetSitePolices", "There was an error processing the request. Exception: {0}", _ex);
                HttpResponseMessage _response = Request.CreateResponse(HttpStatusCode.InternalServerError, _message);
                throw new HttpResponseException(_response); 
            }
        }
    }
}
