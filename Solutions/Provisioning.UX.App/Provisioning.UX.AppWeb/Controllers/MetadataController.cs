using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using OfficeDevPnP.Core.WebAPI;
using Provisioning.Common;
using Provisioning.Common.Data;
using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.Common.Data.Metadata;
using Provisioning.Common.Metadata;
using Provisioning.Common.Utilities;
using Provisioning.UX.AppWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Provisioning.UX.AppWeb.Controllers
{
    public class MetadataController : ApiController
    {
        const string SITE_PROPERTY_DIVISION = "_site_props_division";
        const string SITE_PROPERTY_REGION = "_site_props_region";
        const string SITE_PROPERTY_FUNCTION = "_site_props_function";
        const string SITE_PROPERTY_ISONPREM = "_site_props_sponprem";
        const string SITE_PROPERTY_EXTERNAL_SHARING = "_site_props_externalsharing";

        #region ISharePointClientService
        public void UsingContext(Action<ClientContext> action)
        {
            UsingContext(action, Timeout.Infinite);
        }

        public void UsingContext(Action<ClientContext> action, int csomTimeout)
        {
            using (ClientContext _ctx = this.Authentication.GetAuthenticatedContext())
            {
                _ctx.RequestTimeout = csomTimeout;
                action(_ctx);
            }
        }
        #endregion
        
        public string ConnectionString
        {
            get;
            set;
        }

        public IAuthentication Authentication
        {
            get
            {
                var _auth = new AppOnlyAuthenticationSite();
                _auth.SiteUrl = this.ConnectionString;
                return _auth;
            }
        }

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
        public Models.SiteMetadata GetSitePolices([FromBody]string value)
        {
            SiteEditMetadata _metadata = new SiteEditMetadata();
            var _request = JsonConvert.DeserializeObject<Models.SiteMetadata>(value);
            _request.Success = false;

            _metadata.TenantAdminUrl = _request.TenantAdminUrl;
            _metadata.Url = _request.Url;

            try
            {
                IMetadataFactory _factory = MetadataFactory.GetInstance();
                IMetadataManager _manager = _factory.GetManager();
                var _results = _manager.SetSitePolicy(_metadata);
                _request.Success = true;
                return _request;
            }
            catch (Exception _ex)
            {
                _request.ErrorMessage = _ex.Message;
                OfficeDevPnP.Core.Diagnostics.Log.Error("MetadataController.GetSiteMetadata",
                   "There was an error processing the request. Exception: {0}",
                   _ex);
                return _request;
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

        [Route("api/provisioning/metadata/getBusinessUnits")]
        [HttpGet]
        [WebAPIContextFilter]
        public HttpResponseMessage GetBusinessUnits()
        {
            try
            {
                IMetadataFactory _factory = MetadataFactory.GetInstance();
                IMetadataManager _manager = _factory.GetManager();
                var _metadata = _manager.GetAvailableBusinessUnits();
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
                
        [Route("api/siteedit/metadata/getSiteMetadata")]
        [HttpPost]
        [WebAPIContextFilter]
        public Models.SiteMetadata GetSiteMetadata([FromBody]string value)
        {
            ConfigManager _cfgmanager = new ConfigManager();
            var _auth = new AppOnlyAuthenticationTenant();
            _auth.TenantAdminUrl = _cfgmanager.GetAppSettingsKey("TenantAdminUrl");
            
            var _request = JsonConvert.DeserializeObject<Models.SiteMetadata>(value);
            _request.TenantAdminUrl = _auth.TenantAdminUrl;

            // Transform the request
            var _metadataRequest = ObjectMapper.ToSiteEditMetadata(_request);
            _metadataRequest.Success = false;            

            try
            {
                IMetadataFactory _factory = MetadataFactory.GetInstance();
                IMetadataManager _manager = _factory.GetManager();
                var _metadataResponse = _manager.GetSiteMetadata(_metadataRequest);
                _metadataResponse.Success = true;

                // Transform response 
                var _response = ObjectMapper.ToSiteMetadata(_metadataResponse);
                return _response;
            }
            catch (Exception _ex)
            {
                _request.ErrorMessage = _ex.Message;
                OfficeDevPnP.Core.Diagnostics.Log.Error("MetadataController.GetSiteMetadata",
                   "There was an error processing the request. Exception: {0}",
                   _ex);
                return _request;
            }
        }

        [Route("api/siteedit/metadata/setSiteMetadata")]
        [HttpPost]
        [WebAPIContextFilter]
        public Models.SiteMetadata SetSiteMetadata([FromBody]string value)
        {
            ConfigManager _cfgmanager = new ConfigManager();
            var _auth = new AppOnlyAuthenticationTenant();
            _auth.TenantAdminUrl = _cfgmanager.GetAppSettingsKey("TenantAdminUrl");

            var _request = JsonConvert.DeserializeObject<Models.SiteMetadata>(value);
            _request.TenantAdminUrl = _auth.TenantAdminUrl;

            // Transform the request
            var _metadataRequest = ObjectMapper.ToSiteEditMetadata(_request);
            _metadataRequest.Success = false;

            try
            {
                IMetadataFactory _factory = MetadataFactory.GetInstance();
                IMetadataManager _manager = _factory.GetManager();
                var _metadataResponse = _manager.SetSiteMetadata(_metadataRequest);
                _metadataResponse.Success = true;

                // Transform response 
                var _response = ObjectMapper.ToSiteMetadata(_metadataResponse);
                return _response;
            }
            catch (Exception _ex)
            {
                _request.ErrorMessage = _ex.Message;
                OfficeDevPnP.Core.Diagnostics.Log.Error("MetadataController.SetSiteMetadata",
                   "There was an error processing the request. Exception: {0}",
                   _ex);
                return _request;
            }
        }

    }
}
