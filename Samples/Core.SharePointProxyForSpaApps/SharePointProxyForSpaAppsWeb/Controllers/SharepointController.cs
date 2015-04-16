using SharePointProxyForSpaAppsWeb.Models;
using SharePointProxyForSpaAppsWeb.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SharePointProxyForSpaAppsWeb.Controllers
{
    public class SharepointController : ApiController
    {
        // GET: api/Sharepoint
        public IHttpActionResult Get()
        {
            var data = new string[] { "value1", "value2" };

            return Ok(data);
        }

        // POST: api/Sharepoint
        public async Task<IHttpActionResult> Post([FromBody]Request request)
        {
            SharePointAcsContext spContext;
            try
            {
                spContext = HttpContext.Current.Session["SPContext"] as SharePointAcsContext;
            }
            catch(Exception e)
            {
                return InternalServerError(e);
            }

            if (spContext == null)
            {
                return BadRequest("Unable to get Sharepoint context from current session. Please try reloading the app from SharePoint to create a new context.");
            }

            var httpRequestMessage = Util.ConvertToHttpRequestMessage(request);
            var accessToken = Util.GetAccessTokenRequiredForRequest(httpRequestMessage, spContext);
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var httpClient = new HttpClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            return ResponseMessage(httpResponseMessage);
        }
    }
}
