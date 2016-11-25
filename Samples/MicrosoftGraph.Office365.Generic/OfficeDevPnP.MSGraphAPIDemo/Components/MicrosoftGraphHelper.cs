using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web;
using Microsoft.Owin.Security.OpenIdConnect;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace OfficeDevPnP.MSGraphAPIDemo.Components
{
    public static class MicrosoftGraphHelper
    {
        public static String MicrosoftGraphV1BaseUri = "https://graph.microsoft.com/v1.0/";
        public static String MicrosoftGraphBetaBaseUri = "https://graph.microsoft.com/beta/";

        /// <summary>
        /// This helper method returns and OAuth Access Token for the current user
        /// </summary>
        /// <param name="resourceId">The resourceId for which we are requesting the token</param>
        /// <returns>The OAuth Access Token value</returns>
        public static String GetAccessTokenForCurrentUser(String resourceId = null)
        {
            String accessToken = null;
            if (String.IsNullOrEmpty(resourceId))
            {
                resourceId = MSGraphAPIDemoSettings.MicrosoftGraphResourceId;
            }

            try
            {
                ClientCredential credential = new ClientCredential(
                    MSGraphAPIDemoSettings.ClientId,
                    MSGraphAPIDemoSettings.ClientSecret);
                string signedInUserID = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(
                    ClaimTypes.NameIdentifier).Value;
                AuthenticationContext authContext = new AuthenticationContext(
                    MSGraphAPIDemoSettings.Authority,
                    new SessionADALCache(signedInUserID));

                AuthenticationResult result = authContext.AcquireTokenSilent(
                    resourceId,
                    credential,
                    UserIdentifier.AnyUser);

                accessToken = result.AccessToken;
            }
            catch (AdalException ex)
            {
                if (ex.ErrorCode == "failed_to_acquire_token_silently")
                {
                    // Refresh the access token from scratch
                    HttpContext.Current.GetOwinContext().Authentication.Challenge(
                        new AuthenticationProperties
                        {
                            RedirectUri = HttpContext.Current.Request.Url.ToString(),
                        },
                        OpenIdConnectAuthenticationDefaults.AuthenticationType);
                }
                else
                {
                    // Rethrow the exception
                    throw ex;
                }
            }

            return (accessToken);
        }

        /// <summary>
        /// This helper method makes an HTTP GET request and returns the result as a String
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <returns>The String value of the result</returns>
        public static String MakeGetRequestForString(String requestUrl)
        {
            return (MakeHttpRequest<String>("GET",
                requestUrl,
                resultPredicate: r => r.Content.ReadAsStringAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP GET request and returns the result as a String
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="accept">The accept header for the response</param>
        /// <returns>The Stream  of the result</returns>
        public static System.IO.Stream MakeGetRequestForStream(String requestUrl,
            String accept)
        {
            return (MakeHttpRequest<System.IO.Stream>("GET",
                requestUrl,
                resultPredicate: r => r.Content.ReadAsStreamAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP POST request without a response
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content/type of the request</param>
        public static void MakePostRequest(String requestUrl,
            Object content = null,
            String contentType = null)
        {
            MakeHttpRequest<String>("POST",
                requestUrl,
                content: content,
                contentType: contentType);
        }

        /// <summary>
        /// This helper method makes an HTTP POST request and returns the result as a String
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content/type of the request</param>
        /// <returns>The String value of the result</returns>
        public static String MakePostRequestForString(String requestUrl,
            Object content = null,
            String contentType = null)
        {
            return (MakeHttpRequest<String>("POST",
                requestUrl,
                content: content,
                contentType: contentType,
                resultPredicate: r => r.Content.ReadAsStringAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP PUT request without a response
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content/type of the request</param>
        public static void MakePutRequest(String requestUrl,
            Object content = null,
            String contentType = null)
        {
            MakeHttpRequest<String>("PUT",
                requestUrl,
                content: content,
                contentType: contentType);
        }

        /// <summary>
        /// This helper method makes an HTTP PUT request and returns the result as a String
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content/type of the request</param>
        /// <returns>The String value of the result</returns>
        public static String MakePutRequestForString(String requestUrl,
            Object content = null,
            String contentType = null)
        {
            return(MakeHttpRequest<String>("PUT",
                requestUrl,
                content: content,
                contentType: contentType,
                resultPredicate: r => r.Content.ReadAsStringAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP PATCH request and returns the result as a String
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content/type of the request</param>
        /// <returns>The String value of the result</returns>
        public static String MakePatchRequestForString(String requestUrl,
            Object content = null,
            String contentType = null)
        {
            return (MakeHttpRequest<String>("PATCH",
                requestUrl,
                content: content,
                contentType: contentType,
                resultPredicate: r => r.Content.ReadAsStringAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP DELETE request
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <returns>The String value of the result</returns>
        public static void MakeDeleteRequest(String requestUrl)
        {
            MakeHttpRequest<String>("DELETE", requestUrl);
        }

        /// <summary>
        /// This helper method makes an HTTP request and eventually returns a result
        /// </summary>
        /// <param name="httpMethod">The HTTP method for the request</param>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="accept">The content type of the accepted response</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content  type of the request</param>
        /// <param name="resultPredicate">The predicate to retrieve the result, if any</param>
        /// <typeparam name="TResult">The type of the result, if any</typeparam>
        /// <returns>The value of the result, if any</returns>
        private static TResult MakeHttpRequest<TResult>(
            String httpMethod,
            String requestUrl,
            String accept = null,
            Object content = null,
            String contentType = null,
            Func<HttpResponseMessage, TResult> resultPredicate = null)
        {
            // Prepare the variable to hold the result, if any
            TResult result = default(TResult);

            // Get the OAuth Access Token
            Uri requestUri = new Uri(requestUrl);
            Uri graphUri = new Uri(MSGraphAPIDemoSettings.MicrosoftGraphResourceId);
            var accessToken =
                GetAccessTokenForCurrentUser(requestUri.DnsSafeHost != graphUri.DnsSafeHost ?
                    ($"{requestUri.Scheme}://{requestUri.Host}") : MSGraphAPIDemoSettings.MicrosoftGraphResourceId
                );

            if (!String.IsNullOrEmpty(accessToken))
            {
                // If we have the token, then handle the HTTP request
                HttpClientHandler handler = new HttpClientHandler();
                handler.AllowAutoRedirect = true;
                HttpClient httpClient = new HttpClient(handler, true);

                // Set the Authorization Bearer token
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                // If there is an accept argument, set the corresponding HTTP header
                if (!String.IsNullOrEmpty(accept))
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue(accept));
                }

                // Prepare the content of the request, if any
                HttpContent requestContent = null;
                System.IO.Stream streamContent = content as System.IO.Stream;
                if (streamContent != null)
                {
                    requestContent = new StreamContent(streamContent);
                    requestContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }
                else
                {
                    requestContent =
                        (content != null) ?
                        new StringContent(JsonConvert.SerializeObject(content,
                            Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore,
                                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            }),
                        Encoding.UTF8, contentType) :
                        null;
                }

                // Prepare the HTTP request message with the proper HTTP method
                HttpRequestMessage request = new HttpRequestMessage(
                    new HttpMethod(httpMethod), requestUrl);

                // Set the request content, if any
                if (requestContent != null)
                {
                    request.Content = requestContent;
                }

                // Fire the HTTP request
                HttpResponseMessage response = httpClient.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    // If the response is Success and there is a
                    // predicate to retrieve the result, invoke it
                    if (resultPredicate != null)
                    {
                        result = resultPredicate(response);
                    }
                }
                else
                {
                    throw new ApplicationException(
                        String.Format("Exception while invoking endpoint {0}.", requestUrl),
                        new HttpException(
                            (Int32)response.StatusCode,
                            response.Content.ReadAsStringAsync().Result));
                }
            }

            return (result);
        }
    }
}