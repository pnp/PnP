using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace SPOGraphConsumer.Components
{
    /// <summary>
    /// Static class full of helper methods to make HTTP requests
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// This helper method makes an HTTP GET request and returns the result as a String
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="accessToken">The OAuth 2.0 Access Token for the request, if authorization is required</param>
        /// <returns>The String value of the result</returns>
        public static String MakeGetRequestForString(String requestUrl,
            String accessToken = null)
        {
            return (MakeHttpRequest<String>("GET",
                requestUrl,
                accessToken,
                resultPredicate: r => r.Content.ReadAsStringAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP GET request and returns the result as a String
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="accessToken">The OAuth 2.0 Access Token for the request, if authorization is required</param>
        /// <param name="accept">The accept header for the response</param>
        /// <returns>The Stream  of the result</returns>
        public static System.IO.Stream MakeGetRequestForStream(String requestUrl,
            String accept,
            String accessToken = null,
            String referer = null)
        {
            return (MakeHttpRequest<System.IO.Stream>("GET",
                requestUrl,
                accessToken,
                referer: referer,
                resultPredicate: r => r.Content.ReadAsStreamAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP GET request and returns the result as a String
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="responseHeaders">The response headers of the HTTP request (output argument)</param>
        /// <param name="accessToken">The OAuth 2.0 Access Token for the request, if authorization is required</param>
        /// <param name="accept">The accept header for the response</param>
        /// <returns>The Stream  of the result</returns>
        public static System.IO.Stream MakeGetRequestForStreamWithResponseHeaders(String requestUrl,
            String accept,
            out HttpResponseHeaders responseHeaders,
            String accessToken = null)
        {
            return (MakeHttpRequest<System.IO.Stream>("GET",
                requestUrl,
                out responseHeaders,
                accessToken,
                resultPredicate: r => r.Content.ReadAsStreamAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP POST request without a response
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="accessToken">The OAuth 2.0 Access Token for the request, if authorization is required</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content/type of the request</param>
        public static void MakePostRequest(String requestUrl,
            Object content = null,
            String contentType = null,
            String accessToken = null)
        {
            MakeHttpRequest<String>("POST",
                requestUrl,
                accessToken: accessToken,
                content: content,
                contentType: contentType);
        }

        /// <summary>
        /// This helper method makes an HTTP POST request and returns the result as a String
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="accessToken">The OAuth 2.0 Access Token for the request, if authorization is required</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content/type of the request</param>
        /// <returns>The String value of the result</returns>
        public static String MakePostRequestForString(String requestUrl,
            Object content = null,
            String contentType = null,
            String accessToken = null)
        {
            return (MakeHttpRequest<String>("POST",
                requestUrl,
                accessToken: accessToken,
                content: content,
                contentType: contentType,
                resultPredicate: r => r.Content.ReadAsStringAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP PUT request without a response
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="accessToken">The OAuth 2.0 Access Token for the request, if authorization is required</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content/type of the request</param>
        public static void MakePutRequest(String requestUrl,
            Object content = null,
            String contentType = null,
            String accessToken = null)
        {
            MakeHttpRequest<String>("PUT",
                requestUrl,
                accessToken: accessToken,
                content: content,
                contentType: contentType);
        }

        /// <summary>
        /// This helper method makes an HTTP PUT request and returns the result as a String
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="accessToken">The OAuth 2.0 Access Token for the request, if authorization is required</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content/type of the request</param>
        /// <returns>The String value of the result</returns>
        public static String MakePutRequestForString(String requestUrl,
            Object content = null,
            String contentType = null,
            String accessToken = null)
        {
            return (MakeHttpRequest<String>("PUT",
                requestUrl,
                accessToken: accessToken,
                content: content,
                contentType: contentType,
                resultPredicate: r => r.Content.ReadAsStringAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP PATCH request and returns the result as a String
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="accessToken">The OAuth 2.0 Access Token for the request, if authorization is required</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content/type of the request</param>
        /// <returns>The String value of the result</returns>
        public static String MakePatchRequestForString(String requestUrl,
            Object content = null,
            String contentType = null,
            String accessToken = null)
        {
            return (MakeHttpRequest<String>("PATCH",
                requestUrl,
                accessToken: accessToken,
                content: content,
                contentType: contentType,
                resultPredicate: r => r.Content.ReadAsStringAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP DELETE request
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="accessToken">The OAuth 2.0 Access Token for the request, if authorization is required</param>
        /// <returns>The String value of the result</returns>
        public static void MakeDeleteRequest(String requestUrl,
            String accessToken = null)
        {
            MakeHttpRequest<String>("DELETE", requestUrl, accessToken);
        }

        /// <summary>
        /// This helper method makes an HTTP request and eventually returns a result
        /// </summary>
        /// <param name="httpMethod">The HTTP method for the request</param>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="accessToken">The OAuth 2.0 Access Token for the request, if authorization is required</param>
        /// <param name="accept">The content type of the accepted response</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content  type of the request</param>
        /// <param name="resultPredicate">The predicate to retrieve the result, if any</param>
        /// <typeparam name="TResult">The type of the result, if any</typeparam>
        /// <returns>The value of the result, if any</returns>
        private static TResult MakeHttpRequest<TResult>(
            String httpMethod,
            String requestUrl,
            String accessToken = null,
            String accept = null,
            Object content = null,
            String contentType = null,
            String referer = null,
            Func<HttpResponseMessage, TResult> resultPredicate = null)
        {
            HttpResponseHeaders responseHeaders;
            return (MakeHttpRequest<TResult>(httpMethod,
                requestUrl,
                out responseHeaders,
                accessToken,
                accept,
                content,
                contentType,
                referer,
                resultPredicate));
        }

        /// <summary>
        /// This helper method makes an HTTP request and eventually returns a result
        /// </summary>
        /// <param name="httpMethod">The HTTP method for the request</param>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="responseHeaders">The response headers of the HTTP request (output argument)</param>
        /// <param name="accessToken">The OAuth 2.0 Access Token for the request, if authorization is required</param>
        /// <param name="accept">The content type of the accepted response</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content  type of the request</param>
        /// <param name="resultPredicate">The predicate to retrieve the result, if any</param>
        /// <typeparam name="TResult">The type of the result, if any</typeparam>
        /// <returns>The value of the result, if any</returns>
        private static TResult MakeHttpRequest<TResult>(
            String httpMethod,
            String requestUrl,
            out HttpResponseHeaders responseHeaders,
            String accessToken = null,
            String accept = null,
            Object content = null,
            String contentType = null,
            String referer = null,
            Func<HttpResponseMessage, TResult> resultPredicate = null)
        {
            // Prepare the variable to hold the result, if any
            TResult result = default(TResult);
            responseHeaders = null;

            Uri requestUri = new Uri(requestUrl);

            // If we have the token, then handle the HTTP request
            HttpClientHandler handler = new HttpClientHandler();
            handler.AllowAutoRedirect = true;
            HttpClient httpClient = new HttpClient(handler, true);

            // Set the Authorization Bearer token
            if (!String.IsNullOrEmpty(accessToken))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
            }

            if (!String.IsNullOrEmpty(referer))
            {
                httpClient.DefaultRequestHeaders.Referrer = new Uri(referer);
            }

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

                // Get any response header and put it in the answer
                responseHeaders = response.Headers;
            }
            else
            {
                throw new ApplicationException(
                    String.Format("Exception while invoking endpoint {0}.", requestUrl),
                    new HttpException(
                        (Int32)response.StatusCode,
                        response.Content.ReadAsStringAsync().Result));
            }

            return (result);
        }

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
                resourceId = GraphSettings.GraphResourceId;
            }

            try
            {
                ClientCredential credential = new ClientCredential(
                    GraphSettings.ClientId,
                    GraphSettings.ClientSecret);
                string signedInUserID = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(
                    ClaimTypes.NameIdentifier).Value;
                string tenantId = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(
                    "http://schemas.microsoft.com/identity/claims/tenantid").Value;
                AuthenticationContext authContext = new AuthenticationContext(
                    GraphSettings.AadInstance + GraphSettings.TenantId,
                    new MemoryADALCache(signedInUserID));

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
                    ForceOAuthChallenge();
                }
                else
                {
                    // Rethrow the exception
                    throw ex;
                }
            }

            return (accessToken);
        }

        private static void ForceOAuthChallenge()
        {
            HttpContext.Current.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = HttpContext.Current.Request.Url.ToString(),
                },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }
    }
}