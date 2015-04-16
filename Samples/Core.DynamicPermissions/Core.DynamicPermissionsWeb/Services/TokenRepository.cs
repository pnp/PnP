using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Core.DynamicPermissionsWeb.Services
{
    public class TokenRepository : ITokenRepository
    {
        private HttpRequestBase _request;
        private HttpResponseBase _response;
        public TokenRepository(HttpRequestBase request, HttpResponseBase response)
        {
            _request = request;
            _response = response;
        }



        private Microsoft.SharePoint.Client.ClientContext GetClientContext(Uri sharePointSiteUrl)
        {
            string refreshToken = TokenCache.GetCachedRefreshToken(_request.Cookies);
            string accessToken = TokenHelper.GetAccessToken(refreshToken, TokenHelper.SharePointPrincipal, sharePointSiteUrl.Authority, TokenHelper.GetRealmFromTargetUrl(sharePointSiteUrl)).AccessToken;
            return TokenHelper.GetClientContextWithAccessToken(sharePointSiteUrl.ToString(), accessToken);
        }


        public Uri GetHostUrl()
        {
            HttpCookie spHostUrlCookie = _request.Cookies["SPHostUrl"];
            if (null != spHostUrlCookie)
            {
                return new Uri(spHostUrlCookie.Value);
            }
            else
            {
                return null;
            }
        }

        

        public bool IsConnectedToO365
        {
            get
            {
                bool ret = false;

                Uri sharePointSiteUrl = GetHostUrl();
                if(null != sharePointSiteUrl)
                {
                    ret = TokenCache.IsTokenInCache(_request.Cookies);
                }
                return ret;
            }
        }

        public string GetSiteTitle()
        {
            if(IsConnectedToO365)
            { 
                using (ClientContext context = GetClientContext(GetHostUrl()))
                {
                    context.Load(context.Web);
                    context.ExecuteQuery();

                    return context.Web.Title;
                }
            }
            else { return null; }
        }

        public void Connect(string hostUrl)
        {
            if (!IsConnectedToO365)
            {
                HttpCookie spHostUrlCookie = new HttpCookie("SPHostUrl");
                spHostUrlCookie.Value = hostUrl;
                spHostUrlCookie.Expires = DateTime.Now.AddYears(5);
                _response.Cookies.Add(spHostUrlCookie);
                _response.Redirect(TokenHelper.GetAuthorizationUrl(hostUrl, "Web.Manage"));
            }
        }

        public void Callback(string code)
        {
            HttpCookie spHostUrlCookie = _request.Cookies["SPHostUrl"];
            if (null != spHostUrlCookie)
            {
                Uri sharePointSiteUrl = new Uri(spHostUrlCookie.Value);
                TokenCache.UpdateCacheWithCode(_request, _response, sharePointSiteUrl);
            }
        }

        public void CreateList(string title)
        {
            if (IsConnectedToO365)
            {
                using (ClientContext context = GetClientContext(GetHostUrl()))
                {
                    ListCreationInformation lci = new ListCreationInformation
                    {
                        Title = title,
                        TemplateType = (int)ListTemplateType.Announcements
                    };

                    context.Web.Lists.Add(lci);                    
                    context.ExecuteQuery();
                }
            }            
        }

        public List<string> GetLists()
        {
            List<string> ret = null;

            if (IsConnectedToO365)
            {
                using (ClientContext context = GetClientContext(GetHostUrl()))
                {

                    ListCollection lists = context.Web.Lists;
                    context.Load(lists);
                    context.ExecuteQuery();
                    ret = new List<string>();
                    foreach (var item in lists)
                    {
                        ret.Add(item.Title);
                    }
                }
                
            }
            return ret;
        }
        
    }
}