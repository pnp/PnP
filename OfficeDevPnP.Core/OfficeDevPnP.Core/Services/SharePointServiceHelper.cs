using Microsoft.IdentityModel.S2S.Protocols.OAuth2;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
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
    public class SharePointServiceHelper
    {
        public const string SERVICES_TOKEN = "servicesToken";

        public static bool HasCacheEntry(HttpControllerContext httpControllerContext)
        {
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

        public static ClientContext GetClientContext(HttpControllerContext httpControllerContext)
        {
            CookieHeaderValue cookie = httpControllerContext.Request.Headers.GetCookies(SERVICES_TOKEN).FirstOrDefault();
            if (cookie != null)
            {
                String cacheKey = cookie[SERVICES_TOKEN].Value;
                SharePointServiceContexCacheItem cacheItem = SharePointServiceContextCache.Instance.Get(cacheKey);

                //request a new access token from ACS whenever our current access token will expire in less than 1 hour
                if (cacheItem.AccessToken.ExpiresOn < (DateTime.Now.AddHours(-1)))
                {
                    Uri targetUri = new Uri(cacheItem.SharePointServiceContext.HostWebUrl);
                    OAuth2AccessTokenResponse accessToken = TokenHelper.GetAccessToken(cacheItem.RefreshToken, TokenHelper.SharePointPrincipal, targetUri.Authority, TokenHelper.GetRealmFromTargetUrl(targetUri));
                    cacheItem.AccessToken = accessToken;
                }
                 
                return TokenHelper.GetClientContextWithAccessToken(cacheItem.SharePointServiceContext.HostWebUrl, cacheItem.AccessToken.AccessToken);
            }
            else
            {
                return null;
            }            
        }

        public static void AddToCache(SharePointServiceContext sharePointServiceContext)
        {
            try
            {
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
                SharePointServiceContextCache.Instance.Add(sharePointServiceContext.CacheKey, cacheItem);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public static async void RegisterService(Page page, Uri serviceEndPoint, string apiRequest, String cacheKey)
        {
            if (!page.IsPostBack)
            {
                if (page.Request.QueryString.AsString(SERVICES_TOKEN, string.Empty).Equals(string.Empty))
                {
                    //Remove the = in the cacheKey
                    cacheKey = cacheKey.Replace("=", "");

                    // Write the cachekey in a cookie
                    System.Web.HttpCookie cookie = new HttpCookie(SERVICES_TOKEN)
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
                        Token = TokenHelper.GetContextTokenFromRequest(page.Request),
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
                            throw new Exception(String.Format("Service registration failed: {0}", response.StatusCode));
                        }
                    }
                }
            }
        }

    }
}
