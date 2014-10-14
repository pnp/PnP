using Microsoft.IdentityModel.S2S.Protocols.OAuth2;
using Microsoft.IdentityModel.S2S.Tokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.UI;

namespace OfficeDevPnP.Core.Services
{
    /// <summary>
    /// This class provides helper methods that can be used to protect WebAPI services and to provide a 
    /// way to reinstantiate a contextobject in the service call.
    /// </summary>
    public class SharePointServiceHelper
    {
        /// <summary>
        /// This is the name of the cookie that will hold the cachekey.
        /// </summary>
        public const string SERVICES_TOKEN = "servicesToken";

        /// <summary>
        /// Checks if this request has a servicesToken cookie. To be used from inside the WebAPI.
        /// </summary>
        /// <param name="httpControllerContext">Information about the HTTP request that reached the WebAPI controller</param>
        /// <returns>True if cookie is available and not empty, false otherwise</returns>
        public static bool HasCacheEntry(HttpControllerContext httpControllerContext)
        {
            if (httpControllerContext == null)
                throw new ArgumentNullException("httpControllerContext");

            CookieHeaderValue cookie = httpControllerContext.Request.Headers.GetCookies(SERVICES_TOKEN).FirstOrDefault();
            if (cookie != null && !String.IsNullOrEmpty(cookie[SERVICES_TOKEN].Value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a ClientContext token for the incoming WebAPI request. This is done by 
        /// - looking up the servicesToken
        /// - extracting the cacheKey 
        /// - get the AccessToken from cache. If the AccessToken is expired a new one is requested using the refresh token
        /// - creation of a ClientContext object based on the AccessToken
        /// </summary>
        /// <param name="httpControllerContext">Information about the HTTP request that reached the WebAPI controller</param>
        /// <returns>A valid ClientContext object</returns>
        public static ClientContext GetClientContext(HttpControllerContext httpControllerContext)
        {
            if (httpControllerContext == null)
                throw new ArgumentNullException("httpControllerContext");

            CookieHeaderValue cookie = httpControllerContext.Request.Headers.GetCookies(SERVICES_TOKEN).FirstOrDefault();
            if (cookie != null)
            {
                string cacheKey = cookie[SERVICES_TOKEN].Value;
                SharePointServiceContexCacheItem cacheItem = SharePointServiceContextCache.Instance.Get(cacheKey);

                //request a new access token from ACS whenever our current access token will expire in less than 1 hour
                if (cacheItem.AccessToken.ExpiresOn < (DateTime.Now.AddHours(-1)))
                {
                    Uri targetUri = new Uri(cacheItem.SharePointServiceContext.HostWebUrl);
                    OAuth2AccessTokenResponse accessToken = TokenHelper.GetAccessToken(cacheItem.RefreshToken, TokenHelper.SharePointPrincipal, targetUri.Authority, TokenHelper.GetRealmFromTargetUrl(targetUri));
                    cacheItem.AccessToken = accessToken;
                    //update the cache
                    SharePointServiceContextCache.Instance.Put(cacheKey, cacheItem);
                    LoggingUtility.Internal.TraceInformation((int)EventId.ServicesTokenRefreshed, CoreResources.Services_TokenRefreshed, cacheKey, cacheItem.SharePointServiceContext.HostWebUrl);
                }
                 
                return TokenHelper.GetClientContextWithAccessToken(cacheItem.SharePointServiceContext.HostWebUrl, cacheItem.AccessToken.AccessToken);
            }
            else
            {
                LoggingUtility.Internal.TraceWarning((int)EventId.ServicesNoCachedItem, CoreResources.Services_CookieWithCachKeyNotFound);
                throw new Exception("The cookie with the cachekey was not found...nothing can be retrieved from cache, so no clientcontext can be created.");
            }            
        }

        /// <summary>
        /// Uses the information regarding the requesting app to obtain an access token and caches that using the cachekey.
        /// This method is called from the Register WebAPI service api.
        /// </summary>
        /// <param name="sharePointServiceContext">Object holding information about the requesting SharePoint app</param>
        public static void AddToCache(SharePointServiceContext sharePointServiceContext)
        {
            if (sharePointServiceContext == null)
                throw new ArgumentNullException("sharePointServiceContext");

            TokenHelper.ClientId = sharePointServiceContext.ClientId;
            TokenHelper.HostedAppHostName = sharePointServiceContext.HostedAppHostName;
            SharePointContextToken sharePointContextToken = TokenHelper.ReadAndValidateContextToken(sharePointServiceContext.Token);
            OAuth2AccessTokenResponse accessToken = TokenHelper.GetAccessToken(sharePointContextToken, new Uri(sharePointServiceContext.HostWebUrl).Authority);
            SharePointServiceContexCacheItem cacheItem = new SharePointServiceContexCacheItem()
            {
                RefreshToken = sharePointContextToken.RefreshToken,
                AccessToken = accessToken,
                SharePointServiceContext = sharePointServiceContext
            };
            SharePointServiceContextCache.Instance.Put(sharePointServiceContext.CacheKey, cacheItem);
        }

        /// <summary>
        /// This method needs to be called from a code behind of the SharePoint app startup page (default.aspx). It registers the calling
        /// SharePoint app by calling a specific "Register" api in your WebAPI service.
        /// 
        /// Note:
        /// Given that method is async you'll need to add the  Async="true" page directive to the page that uses this method.
        /// </summary>
        /// <param name="page">The page object, needed to insert the services token cookie and read the querystring</param>
        /// <param name="apiRequest">Route to the "Register" API</param>
        /// <param name="serviceEndPoint">Optional Uri to the WebAPI service endpoint. If null then the assumption is taken that the WebAPI is hosted together with the page making this call</param>
        public static async void RegisterService(Page page, string apiRequest, Uri serviceEndPoint = null)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (string.IsNullOrEmpty(apiRequest))
                throw new ArgumentNullException("apiRequest");

            if (!page.IsPostBack)
            {
                if (page.Request.QueryString.AsString(SERVICES_TOKEN, string.Empty).Equals(string.Empty))
                {
                    // Construct a JsonWebSecurityToken so we can fetch the cachekey...implementation is copied from tokenhelper approach
                    string cacheKey = string.Empty;
                    string contextToken = TokenHelper.GetContextTokenFromRequest(page.Request);
                    JsonWebSecurityTokenHandler tokenHandler = TokenHelper.CreateJsonWebSecurityTokenHandler();
                    SecurityToken securityToken = tokenHandler.ReadToken(contextToken);
                    JsonWebSecurityToken jsonToken = securityToken as JsonWebSecurityToken;
                    string appctx = GetClaimValue(jsonToken, "appctx");
                    if (appctx != null)
                    {
                        ClientContext ctx = new ClientContext("http://tempuri.org");
                        Dictionary<string, object> dict = (Dictionary<string, object>)ctx.ParseObjectFromJsonString(appctx);
                        cacheKey = (string)dict["CacheKey"];
                    }

                    // Remove special chars (=, +, /, {}) from cachekey as there's a flaw in CookieHeaderValue when the 
                    // cookie is read. This flaw replaces special chars with a space.
                    cacheKey = RemoveSpecialCharacters(cacheKey);

                    // Write the cachekey in a cookie
                    HttpCookie cookie = new HttpCookie(SERVICES_TOKEN)
                    {
                        Value = cacheKey,
                        Secure = true,
                        HttpOnly = true,                        
                    };
                    page.Response.AppendCookie(cookie);

                    //Register the ClientContext
                    SharePointServiceContext sharePointServiceContext = new SharePointServiceContext()
                    {
                        CacheKey = cacheKey,
                        ClientId = TokenHelper.ClientId,
                        Token = contextToken,
                        HostWebUrl = page.Request.QueryString.AsString("SPHostUrl", null),
                        AppWebUrl = page.Request.QueryString.AsString("SPAppWebUrl", null),
                        HostedAppHostName = String.Format("{0}:{1}", page.Request.Url.Host, page.Request.Url.Port),
                    };

                    if (serviceEndPoint == null)
                    {
                        serviceEndPoint = new Uri(String.Format("{0}://{1}:{2}", page.Request.Url.Scheme, page.Request.Url.Host, page.Request.Url.Port));
                    }

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = serviceEndPoint;
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.PutAsJsonAsync(apiRequest, sharePointServiceContext);

                        if (!response.IsSuccessStatusCode)
                        {
                            LoggingUtility.Internal.TraceError((int)EventId.ServicesRegistrationFailed, CoreResources.Service_RegistrationFailed, apiRequest, serviceEndPoint.ToString(), cacheKey);
                            throw new Exception(String.Format("Service registration failed: {0}", response.StatusCode));
                        }

                        LoggingUtility.Internal.TraceInformation((int)EventId.ServicesRegistered, CoreResources.Services_Registered, apiRequest, serviceEndPoint.ToString(), cacheKey);

                    }
                }
            }
        }

        private static string GetClaimValue(JsonWebSecurityToken token, string claimType)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            foreach (JsonWebTokenClaim claim in token.Claims)
            {
                if (StringComparer.Ordinal.Equals(claim.ClaimType, claimType))
                {
                    return claim.Value;
                }
            }

            return null;
        }

        private static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

    }
}
