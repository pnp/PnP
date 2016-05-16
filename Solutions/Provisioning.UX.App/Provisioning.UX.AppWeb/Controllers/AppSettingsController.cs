using Newtonsoft.Json;
using OfficeDevPnP.Core.WebAPI;
using Provisioning.Common;
using Provisioning.Common.Data.AppSettings;
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
    public class AppSettingsController : ApiController
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
        [Route("api/provisioning/appSettings/get")]
        [HttpGet]
        [WebAPIContextFilter]
        public HttpResponseMessage GetAppSettings()
        {  
            try
            {
                IAppSettingsFactory _factory = AppSettingsFactory.GetInstance();
                IAppSettingsManager _manager = _factory.GetManager();
                var _appSettings = _manager.GetAppSettings();
                return Request.CreateResponse(HttpStatusCode.OK, _appSettings);
            }
            catch (Exception _ex)
            {
                var _message = string.Format("There was an error processing the request. {0}", _ex.Message);
                Log.Error("AppSettingsController.GetAppSettings", "There was an error processing the request. Exception: {0}", _ex);
                HttpResponseMessage _response = Request.CreateResponse(HttpStatusCode.InternalServerError, _message);
                throw new HttpResponseException(_response); 
            }
        }
    }
}
