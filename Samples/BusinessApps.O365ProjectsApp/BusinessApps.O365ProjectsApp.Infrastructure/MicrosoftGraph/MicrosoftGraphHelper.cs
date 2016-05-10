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
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace BusinessApps.O365ProjectsApp.Infrastructure.MicrosoftGraph
{
    public static class MicrosoftGraphHelper
    {
        public static String MicrosoftGraphV1BaseUri = "https://graph.microsoft.com/v1.0/";
        public static String MicrosoftGraphBetaBaseUri = "https://graph.microsoft.com/beta/";

        /// <summary>
        /// This helper method returns and OAuth Access Token for the current user, if any, 
        /// or App-Only if there is no current user
        /// </summary>
        /// <param name="resourceId">The resourceId for which we are requesting the token</param>
        /// <param name="forceAppOnlyContext">Forces retrieval of an App-Only access token</param>
        /// <returns>The OAuth Access Token value</returns>
        public static async Task<String> GetAccessTokenForCurrentContextAsync(String resourceId = null, Boolean forceAppOnlyContext = false)
        {
            String accessToken = null;

            if (String.IsNullOrEmpty(resourceId))
            {
                resourceId = O365ProjectsAppSettings.MicrosoftGraphResourceId;
            }

            try
            {
                AuthenticationContext authContext = null;
                AuthenticationResult result = null;

                // If we have the current user, and we don't have to force App-Only context
                if (!forceAppOnlyContext &&
                    System.Security.Claims.ClaimsPrincipal.Current != null &&
                    System.Security.Claims.ClaimsPrincipal.Current.Identity != null &&
                    System.Security.Claims.ClaimsPrincipal.Current.Identity.IsAuthenticated)
                {
                    ClientCredential credential = new ClientCredential(
                        O365ProjectsAppSettings.ClientId,
                        O365ProjectsAppSettings.ClientSecret);

                    String signedInUserID = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(
                        ClaimTypes.NameIdentifier).Value;
                    String tenantId = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(
                        "http://schemas.microsoft.com/identity/claims/tenantid").Value;

                    authContext = new AuthenticationContext(
                        O365ProjectsAppSettings.AADInstance + tenantId,
                        new SessionADALCache(signedInUserID));

                    result = await authContext.AcquireTokenSilentAsync(
                        resourceId,
                        credential,
                        UserIdentifier.AnyUser);
                }
                else
                {
                    authContext = new AuthenticationContext(
                        O365ProjectsAppSettings.AADInstance + O365ProjectsAppSettings.TenantId);

                    ClientAssertionCertificate certCredential = new ClientAssertionCertificate(
                        O365ProjectsAppSettings.ClientId,
                        O365ProjectsAppSettings.AppOnlyCertificate);

                    result = await authContext.AcquireTokenAsync(
                        resourceId,
                        certCredential);
                }

                if (result != null)
                {
                    accessToken = result.AccessToken;
                }
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
        /// This helper method returns and OAuth Access Token for the current user
        /// </summary>
        /// <param name="resourceId">The resourceId for which we are requesting the token</param>
        /// <returns>The OAuth Access Token value</returns>
        public static String GetAccessTokenForCurrentUser(String resourceId = null)
        {
            String accessToken = null;

            if (String.IsNullOrEmpty(resourceId))
            {
                resourceId = O365ProjectsAppSettings.MicrosoftGraphResourceId;
            }

            try
            {
                ClientCredential credential = new ClientCredential(
                    O365ProjectsAppSettings.ClientId,
                    O365ProjectsAppSettings.ClientSecret);

                String signedInUserID = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(
                    ClaimTypes.NameIdentifier).Value;
                String tenantId = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(
                    "http://schemas.microsoft.com/identity/claims/tenantid").Value;

                AuthenticationContext authContext = new AuthenticationContext(
                    O365ProjectsAppSettings.AADInstance + tenantId,
                    new SessionADALCache(signedInUserID));

                AuthenticationResult result = authContext.AcquireTokenSilent(
                    resourceId,
                    credential,
                    UserIdentifier.AnyUser);

                if (result != null)
                {
                    accessToken = result.AccessToken;
                }
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
        /// <param name="accessToken">The AccessToken to use for the request</param>
        /// <returns>The String value of the result</returns>
        public static String MakeGetRequestForString(String requestUrl,
            String accessToken = null)
        {
            return (MakeHttpRequest<String>("GET",
                requestUrl,
                accessToken: accessToken,
                resultPredicate: r => r.Content.ReadAsStringAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP GET request and returns the result as a String
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="accept">The accept header for the response</param>
        /// <param name="accessToken">The AccessToken to use for the request</param>
        /// <returns>The Stream  of the result</returns>
        public static System.IO.Stream MakeGetRequestForStream(String requestUrl,
            String accept, String accessToken = null)
        {
            return (MakeHttpRequest<System.IO.Stream>("GET",
                requestUrl,
                accessToken: accessToken,
                resultPredicate: r => r.Content.ReadAsStreamAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP POST request without a response
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content/type of the request</param>
        /// <param name="accessToken">The AccessToken to use for the request</param>
        public static void MakePostRequest(String requestUrl,
            Object content = null,
            String contentType = null,
            String accessToken = null)
        {
            MakeHttpRequest<String>("POST",
                requestUrl,
                content: content,
                contentType: contentType,
                accessToken: accessToken);
        }

        /// <summary>
        /// This helper method makes an HTTP POST request and returns the result as a String
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content/type of the request</param>
        /// <param name="accessToken">The AccessToken to use for the request</param>
        /// <returns>The String value of the result</returns>
        public static String MakePostRequestForString(String requestUrl,
            Object content = null,
            String contentType = null,
            String accessToken = null)
        {
            return (MakeHttpRequest<String>("POST",
                requestUrl,
                content: content,
                contentType: contentType,
                accessToken: accessToken,
                resultPredicate: r => r.Content.ReadAsStringAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP PUT request without a response
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content/type of the request</param>
        /// <param name="accessToken">The AccessToken to use for the request</param>
        public static void MakePutRequest(String requestUrl,
            Object content = null,
            String contentType = null,
            String accessToken = null)
        {
            MakeHttpRequest<String>("PUT",
                requestUrl,
                content: content,
                contentType: contentType,
                accessToken: accessToken);
        }

        /// <summary>
        /// This helper method makes an HTTP PATCH request and returns the result as a String
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="content">The content of the request</param>
        /// <param name="contentType">The content/type of the request</param>
        /// <param name="accessToken">The AccessToken to use for the request</param>
        /// <returns>The String value of the result</returns>
        public static String MakePatchRequestForString(String requestUrl,
            Object content = null,
            String contentType = null,
            String accessToken = null)
        {
            return (MakeHttpRequest<String>("PATCH",
                requestUrl,
                content: content,
                contentType: contentType,
                accessToken: accessToken,
                resultPredicate: r => r.Content.ReadAsStringAsync().Result));
        }

        /// <summary>
        /// This helper method makes an HTTP DELETE request
        /// </summary>
        /// <param name="requestUrl">The URL of the request</param>
        /// <param name="accessToken">The AccessToken to use for the request</param>
        /// <returns>The String value of the result</returns>
        public static void MakeDeleteRequest(String requestUrl,
            String accessToken = null)
        {
            MakeHttpRequest<String>("DELETE", requestUrl, accessToken: accessToken);
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
        /// <param name="accessToken">The AccessToken to use for the request</param>
        /// <typeparam name="TResult">The type of the result, if any</typeparam>
        /// <returns>The value of the result, if any</returns>
        private static TResult MakeHttpRequest<TResult>(
            String httpMethod,
            String requestUrl,
            String accept = null,
            Object content = null,
            String contentType = null,
            String accessToken = null,
            Func<HttpResponseMessage, TResult> resultPredicate = null)
        {
            // Prepare the variable to hold the result, if any
            TResult result = default(TResult);

            // Get the OAuth Access Token
            if (String.IsNullOrEmpty(accessToken))
            {
                Uri requestUri = new Uri(requestUrl);
                Uri graphUri = new Uri(O365ProjectsAppSettings.MicrosoftGraphResourceId);
                var accessTokenAsync =
                    GetAccessTokenForCurrentContextAsync(requestUri.DnsSafeHost != graphUri.DnsSafeHost ?
                        ($"{requestUri.Scheme}://{requestUri.Host}") : O365ProjectsAppSettings.MicrosoftGraphResourceId
                    );

                accessToken = accessTokenAsync.Result;
            }

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
