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
        [WebAPIContextFilter]
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


        [Route("api/provisioning/metadata/getSiteClassifications")]
        [HttpGet]
        [WebAPIContextFilter]
        public HttpResponseMessage GetSiteClassifications()
        {
            try
            {
                IMetadataFactory _factory = MetadataFactory.GetInstance();
                IMetadataManager _manager = _factory.GetManager();
                var _metadata = _manager.GetAvailableSiteClassifications();
                return Request.CreateResponse(HttpStatusCode.OK, _metadata);
            }
            catch (Exception _ex)
            {
                var _message = string.Format("There was an error processing the request. {0}", _ex.Message);
                Log.Error("MetadataController.GetAvailableSiteClassifications", "There was an error processing the request. Exception: {0}", _ex);
                HttpResponseMessage _response = Request.CreateResponse(HttpStatusCode.InternalServerError, _message);
                throw new HttpResponseException(_response);
            }
        }
        [Route("api/provisioning/metadata/getRegions")]
        [HttpGet]
        [WebAPIContextFilter]
        public HttpResponseMessage GetRegions()
        {
            try
            {
                IMetadataFactory _factory = MetadataFactory.GetInstance();
                IMetadataManager _manager = _factory.GetManager();
                var _metadata = _manager.GetAvailableRegions();
                return Request.CreateResponse(HttpStatusCode.OK, _metadata);
            }
            catch (Exception _ex)
            {
                var _message = string.Format("There was an error processing the request. {0}", _ex.Message);
                Log.Error("MetadataController.GetRegions", "There was an error processing the request. Exception: {0}", _ex);
                HttpResponseMessage _response = Request.CreateResponse(HttpStatusCode.InternalServerError, _message);
                throw new HttpResponseException(_response);
            }
        }
        [Route("api/provisioning/metadata/getDivisions")]
        [HttpGet]
        [WebAPIContextFilter]
        public HttpResponseMessage GetDivisions()
        {
            try
            {
                IMetadataFactory _factory = MetadataFactory.GetInstance();
                IMetadataManager _manager = _factory.GetManager();
                var _metadata = _manager.GetAvailableDivisions();
                return Request.CreateResponse(HttpStatusCode.OK, _metadata);
            }
            catch (Exception _ex)
            {
                var _message = string.Format("There was an error processing the request. {0}", _ex.Message);
                Log.Error("MetadataController.GetDivisions", "There was an error processing the request. Exception: {0}", _ex);
                HttpResponseMessage _response = Request.CreateResponse(HttpStatusCode.InternalServerError, _message);
                throw new HttpResponseException(_response);
            }
        }
        [Route("api/provisioning/metadata/getFunctions")]
        [HttpGet]
        [WebAPIContextFilter]
        public HttpResponseMessage GetFunctions()
        {
            try
            {
                IMetadataFactory _factory = MetadataFactory.GetInstance();
                IMetadataManager _manager = _factory.GetManager();
                var _metadata = _manager.GetAvailableOrganizationalFunctions();
                return Request.CreateResponse(HttpStatusCode.OK, _metadata);
            }
            catch (Exception _ex)
            {
                var _message = string.Format("There was an error processing the request. {0}", _ex.Message);
                Log.Error("MetadataController.GetFunctions", "There was an error processing the request. Exception: {0}", _ex);
                HttpResponseMessage _response = Request.CreateResponse(HttpStatusCode.InternalServerError, _message);
                throw new HttpResponseException(_response);
            }
        }
        [Route("api/provisioning/metadata/getLanguages")]
        [HttpGet]
        [WebAPIContextFilter]
        public HttpResponseMessage GetLanguages()
        {
            try
            {
                IMetadataFactory _factory = MetadataFactory.GetInstance();
                IMetadataManager _manager = _factory.GetManager();
                var _metadata = _manager.GetAvailableLanguages();
                return Request.CreateResponse(HttpStatusCode.OK, _metadata);
            }
            catch (Exception _ex)
            {
                var _message = string.Format("There was an error processing the request. {0}", _ex.Message);
                Log.Error("MetadataController.GetLanguages", "There was an error processing the request. Exception: {0}", _ex);
                HttpResponseMessage _response = Request.CreateResponse(HttpStatusCode.InternalServerError, _message);
                throw new HttpResponseException(_response);
            }
        }
        [Route("api/provisioning/metadata/getTimeZones")]
        [HttpGet]
        [WebAPIContextFilter]
        public HttpResponseMessage GetTimeZones()
        {
            try
            {
                IMetadataFactory _factory = MetadataFactory.GetInstance();
                IMetadataManager _manager = _factory.GetManager();
                var _metadata = _manager.GetAvailableTimeZones();
                return Request.CreateResponse(HttpStatusCode.OK, _metadata);
            }
            catch (Exception _ex)
            {
                var _message = string.Format("There was an error processing the request. {0}", _ex.Message);
                Log.Error("MetadataController.GetTimeZones", "There was an error processing the request. Exception: {0}", _ex);
                HttpResponseMessage _response = Request.CreateResponse(HttpStatusCode.InternalServerError, _message);
                throw new HttpResponseException(_response);
            }
        }
    }
}
