using SharePointProxyForSpaAppsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace SharePointProxyForSpaAppsWeb.Utilities
{
    public class Util
    {
        public static HttpRequestMessage ConvertToHttpRequestMessage(Request request)
        {
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = new HttpMethod(request.Method);
            httpRequestMessage.RequestUri = new Uri(request.Url);

            if ( (httpRequestMessage.Method == HttpMethod.Post)
                || (httpRequestMessage.Method == HttpMethod.Put)
            )
            {
                httpRequestMessage.Content = new StringContent(request.Data, System.Text.Encoding.UTF8, "application/json");
            }

            foreach(var kv in request.Headers)
            {
                if (kv.Key == "Accept")
                {
                    httpRequestMessage.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(kv.Value.ToString()));
                }
                else
                {
                    httpRequestMessage.Headers.Add(kv.Key, kv.Value.ToString());
                }
            }

            return httpRequestMessage;
        }

        public static string GetAccessTokenRequiredForRequest(HttpRequestMessage httpRequestMessage, SharePointAcsContext spContext)
        {
            string accessToken = null;

            if (httpRequestMessage.RequestUri.AbsoluteUri.StartsWith(spContext.SPAppWebUrl.AbsoluteUri))
            {
                accessToken = spContext.UserAccessTokenForSPAppWeb;
            }
            else if (httpRequestMessage.RequestUri.AbsoluteUri.StartsWith(spContext.SPHostUrl.AbsoluteUri))
            {
                accessToken = spContext.UserAccessTokenForSPHost;
            }

            if (String.IsNullOrEmpty(accessToken))
            {
                throw new Exception("Request url did not target the host web or appweb, unable to continue proxying request. The request url is misformatted or you should not be using the proxy.");
            }

            return accessToken;
        }
    }
}