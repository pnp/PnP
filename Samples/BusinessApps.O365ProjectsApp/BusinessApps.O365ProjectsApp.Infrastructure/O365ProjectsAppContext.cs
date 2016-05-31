using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BusinessApps.O365ProjectsApp.Infrastructure
{
    public static class O365ProjectsAppContext
    {
        public static String CurrentUserUPN
        {
            get
            {
                String result = null;

                var upn = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(System.Security.Claims.ClaimTypes.Upn);
                if (upn != null)
                {
                    result = upn.Value;
                }

                return (result);
            }
        }

        public static String CurrentUserDisplayName
        {
            get
            {
                String result = null;

                var name = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("name");
                if (name != null)
                {
                    result = name.Value;
                }

                return (result);
            }
        }

        public static String CurrentUserInitials
        {
            get
            {
                String result = null;

                var name = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("name");
                if (name != null)
                {
                    var nameParts = name.Value.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (nameParts.Length >= 2)
                    {
                        result = nameParts[0].Substring(0, 1) + nameParts[1].Substring(0, 1);
                    }
                    else
                    {
                        result = nameParts[0].Substring(0, 2);
                    }
                }

                return (result.ToUpper());
            }
        }

        public static Boolean CurrentUserIsAdmin
        {
            get
            {
                return (true);
            }
        }

        public static String CurrentSiteUrl
        {
            get
            {
                var siteUrl = HttpContext.Current.Request["SiteUrl"];
                if (!String.IsNullOrEmpty(siteUrl))
                {
                    return (siteUrl);
                }
                else
                {
                    return (O365ProjectsAppSettings.DefaultSiteUrl);
                }
            }
        }

        public static String CurrentAppSiteUrl
        {
            get
            {
                Uri appSiteUrl = new Uri($"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}:{HttpContext.Current.Request.Url.Port}/");
                return (appSiteUrl.ToString());
            }
        }
    }
}