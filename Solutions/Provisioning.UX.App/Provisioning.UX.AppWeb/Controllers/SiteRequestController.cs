using Newtonsoft.Json;
using OfficeDevPnP.Core.WebAPI;
using Provisioning.Common;
using Provisioning.Common.Data.SiteRequests;
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
    public class SiteRequestController : ApiController
    {
        [HttpPut]
        public void Register(WebAPIContext sharePointServiceContext)
        {
            WebAPIHelper.AddToCache(sharePointServiceContext);
        }

        /// <summary>
        /// Checks if a site request exists in the data repository
        /// POST api/<controller>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Route("api/provisioning/siteRequests/validateNewSiteRequestUrl")]
        [WebAPIContextFilter]
        [HttpPost]
        public SiteCheckResponse ValidateNewSiteRequestUrl([FromBody]string value)
        {
            var _response = new SiteCheckResponse();
            _response.Success = false;

            try
            {
                var data = JsonConvert.DeserializeObject<SiteRequest>(value);
                var _requestToCheck = ObjectMapper.ToSiteRequestInformation(data);

                ///Save the Site Request
                ISiteRequestFactory _srf = SiteRequestFactory.GetInstance();
                var _manager = _srf.GetSiteRequestManager();
                bool _value = _manager.DoesSiteRequestExist(_requestToCheck.Url);
                _response.DoesExist = _value;
                _response.Success = true;
            }
            catch (Exception ex)
            {
                Log.Error("SiteRequestController.ValidateNewSiteRequestUrl",
                    "There was an error processing your request. Error Message {0} Error Stack {1}",
                    ex.Message,
                    ex);
                _response.ErrorMessage = ex.Message;
            }
            return _response;

        }

        /// <summary>
        /// Creates new a site request in the data repository
        /// POST api/<controller>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Route("api/provisioning/siteRequests/newSiteRequest")]
        [WebAPIContextFilter]
        [HttpPost]
        public SiteRequest NewSiteRequest([FromBody]string value)
        {
            var _response = new SiteRequest();
           _response.Success = false;

            try
            {
                var data = JsonConvert.DeserializeObject<SiteRequest>(value);
                var _newRequest = ObjectMapper.ToSiteRequestInformation(data);

                ///Save the Site Request
                ISiteRequestFactory _srf = SiteRequestFactory.GetInstance();
                var _manager = _srf.GetSiteRequestManager();
                _manager.CreateNewSiteRequest(_newRequest);
                _response.Success = true;
            }
            catch (Exception ex)
            {
                Log.Error("SiteRequestController.NewSiteRequest",
                    "There was an error saving the new site request. Error Message {0} Error Stack {1}",
                    ex.Message,
                    ex);
                _response.ErrorMessage = ex.Message;
            }
            return _response;

        }

        /// <summary>
        /// Saves a site request to the Data Repository
        /// POST api/<controller>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Route("api/provisioning/siteRequests/getOwnerRequests")]
        [WebAPIContextFilter]
        [HttpPost]
        public SiteRequestsResponse GetOwnerRequestsByEmail([FromBody] string ownerEmailAddress)
        {
            var _returnResponse = new SiteRequestsResponse();
            _returnResponse.Success = false;
            var _user = JsonConvert.DeserializeObject<SiteUser>(ownerEmailAddress);
            try
            {

                ISiteRequestFactory _requestFactory = SiteRequestFactory.GetInstance();
                var _manager = _requestFactory.GetSiteRequestManager();
                _returnResponse.SiteRequests = _manager.GetOwnerRequests(_user.Name); 
                _returnResponse.Success = true;
            }
            catch(Exception _ex)
            {
                _returnResponse.ErrorMessage = _ex.Message;
                Log.Error("SiteRequestController.GetOwnerRequestsByEmail", "There was an error processing the request. Exception: {0}", _ex);
            }

            return _returnResponse;   

        }
    }
}
