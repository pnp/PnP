using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Core.DisplayCalendarEventsWeb.Models;

namespace Core.DisplayCalendarEventsWeb.Utilities {
    public class Http {
        public static HttpRequestMessage ConvertToHttpRequestMessage(Request request) {
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = new HttpMethod(request.Method);
            httpRequestMessage.RequestUri = new Uri(request.Url);

            if (((httpRequestMessage.Method == HttpMethod.Post)
                || (httpRequestMessage.Method == HttpMethod.Put))
                && (request.Data != null)
            ) {
                httpRequestMessage.Content = new StringContent(request.Data.ToString(), System.Text.Encoding.UTF8, "application/json");
            }

            foreach (var kv in request.Headers) {
                if (0 == String.Compare(kv.Key, "Accept", true)) {
                    httpRequestMessage.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(kv.Value.ToString()));
                }
                else if (0 == String.Compare(kv.Key, "Content-Type", true)) {
                    // Take care of this when we create Content object above
                }
                else {
                    httpRequestMessage.Headers.Add(kv.Key, kv.Value.ToString());
                }
            }

            return httpRequestMessage;
        }

        public static string GetAccessTokenRequiredForRequest(HttpRequestMessage httpRequestMessage, SharePointAcsContext spContext) {
            string accessToken = null;

            if ((spContext.SPAppWebUrl != null) && httpRequestMessage.RequestUri.AbsoluteUri.StartsWith(spContext.SPAppWebUrl.AbsoluteUri)) {
                accessToken = spContext.UserAccessTokenForSPAppWeb;
            }
            else if ((spContext.SPHostUrl != null) && httpRequestMessage.RequestUri.AbsoluteUri.StartsWith(spContext.SPHostUrl.AbsoluteUri)) {
                accessToken = spContext.UserAccessTokenForSPHost;
            }
            else {
                accessToken = spContext.CreateAppOnlyClientContextForSPHost().ToString();
            }

            if (String.IsNullOrEmpty(accessToken)) {
                throw new Exception("Request url did not target the host web or appweb, unable to continue proxying request. The request url is misformatted or you should not be using the proxy.");
            }

            return accessToken;
        }
    }
}