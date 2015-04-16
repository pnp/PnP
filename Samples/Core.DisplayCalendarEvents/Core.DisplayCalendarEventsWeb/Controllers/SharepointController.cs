using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Core.DisplayCalendarEventsWeb.Models;
using System.Web;
using System.Net.Http.Headers;

namespace Core.DisplayCalendarEventsWeb.Controllers
{
    public class SharepointController : ApiController
    {
        public IHttpActionResult Get() {
            return Ok("/api/sharepoint exists!");
        }

        public async Task<IHttpActionResult> Post([FromBody]Request request, [FromUri] string SPHostUrl = null) {
            var httpRequestMessage = Utilities.Http.ConvertToHttpRequestMessage(request);
            string accessToken = "";

            // If there is SPHostUrl provided, assume this request is intended to be executed using App only context,
            // otherwise, fallback to normal requests with User + App context which we need to get from the saved SpContext
            if (Uri.IsWellFormedUriString(SPHostUrl, UriKind.Absolute)) {
                // Get app only access token
                var sharepointUrl = new Uri(SPHostUrl);
                var realm = TokenHelper.GetRealmFromTargetUrl(sharepointUrl);
                accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, sharepointUrl.Authority, realm).AccessToken;
            }
            else {
                SharePointAcsContext spContext;
                try {
                    spContext = HttpContext.Current.Session["SPContext"] as SharePointAcsContext;
                }
                catch (Exception e) {
                    return InternalServerError(e);
                }

                if (spContext == null) {
                    return BadRequest("Unable to get Sharepoint context from current session. Please try reloading the app from SharePoint to create a new context.");
                }

                accessToken = Utilities.Http.GetAccessTokenRequiredForRequest(httpRequestMessage, spContext);
            }

            if (String.IsNullOrEmpty(accessToken)) {
                return BadRequest("Access token must not be null.");
            }

            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            return ResponseMessage(httpResponseMessage);
        }
    }
}
