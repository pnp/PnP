namespace Addin1Web
{
    using AddinsConfiguration;
    using AddinsWeb;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Configuration;
    
    /// <summary>
    /// Class ContextHelper.
    /// </summary>
    public static class ContextHelper
    {
        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <returns>The ClientId.</returns>
        public static string GetClientId()
        {
            string currentUrl = HttpContext.Current.Request.RawUrl;

            ////Get the title of the current AddIn using the current url
            AddinSection section = (AddinSection)ConfigurationManager.GetSection("addinSection");
            AddIn addin = section.Addins.Cast<AddIn>().First(a => currentUrl.ToLower().Contains(a.Url.ToLower()));

            ////Return the ClientId corresponding to the current AddIn
            return addin.ClientId;
        }

        /// <summary>
        /// Gets the client secret.
        /// </summary>
        /// <returns>The ClientSecret.</returns>
        public static string GetClientSecret()
        {
            string currentUrl = HttpContext.Current.Request.RawUrl;

            ////Get the title of the current AddIn using the current url
            AddinSection section = (AddinSection)ConfigurationManager.GetSection("addinSection");
            AddIn addin = section.Addins.Cast<AddIn>().First(a => currentUrl.ToLower().Contains(a.Url.ToLower()));

            ////Return the ClientSecret stored in appSettings corresponding to the current AddIn
            return WebConfigurationManager.AppSettings.Get(addin.Name);
        }

        /// <summary>
        /// Loads the SharePoint context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The SharePointContext.</returns>
        public static SharePointContext LoadSharePointContext(HttpContextBase httpContext)
        {
            string currentUrl = HttpContext.Current.Request.RawUrl;

            ////Get the title of the current AddIn using the current url
            AddinSection section = (AddinSection)ConfigurationManager.GetSection("addinSection");
            AddIn addin = section.Addins.Cast<AddIn>().First(a => currentUrl.ToLower().Contains(a.Url.ToLower()));

            return httpContext.Session[addin.Name] as SharePointContext;
        }

        /// <summary>
        /// Loads the SharePoint ACS context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The SharePointContext.</returns>
        public static SharePointAcsContext LoadSharePointAcsContext(HttpContextBase httpContext)
        {
            return LoadSharePointContext(httpContext) as SharePointAcsContext;
        }

        /// <summary>
        /// Loads the SharePoint high trust context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The SharePointContext.</returns>
        public static SharePointHighTrustContext LoadSharePointHighTrustContext(HttpContextBase httpContext)
        {
            return LoadSharePointContext(httpContext) as SharePointHighTrustContext;
        }

        /// <summary>
        /// Saves the SharePoint context.
        /// </summary>
        /// <param name="spContext">The SP context.</param>
        /// <param name="httpContext">The HTTP context.</param>
        public static void SaveSharePointContext(SharePointContext spContext, HttpContextBase httpContext)
        {
            string currentUrl = HttpContext.Current.Request.RawUrl;

            ////Get the title of the current AddIn using the current url
            AddinSection section = (AddinSection)ConfigurationManager.GetSection("addinSection");
            AddIn addin = section.Addins.Cast<AddIn>().First(a => currentUrl.ToLower().Contains(a.Url.ToLower()));

            httpContext.Session[addin.Name] = spContext as SharePointContext;
        }

        /// <summary>
        /// Creates a cookie for the current Add-In by looking at the current URL.
        /// </summary>
        /// <param name="spAcsContext">The SP context.</param>
        /// <returns></returns>
        public static HttpCookie CreateCookieForAddIn(SharePointAcsContext spAcsContext)
        {
            string currentUrl = HttpContext.Current.Request.RawUrl;
            AddinSection section = (AddinSection)ConfigurationManager.GetSection("addinSection");
            AddIn addin = section.Addins.Cast<AddIn>().First(a => currentUrl.ToLower().Contains(a.Url.ToLower()));
            HttpCookie spCacheKeyCookie = new HttpCookie(addin.Name)
            {
                Value = spAcsContext.CacheKey,
                Secure = true,
                HttpOnly = true
            };

            return spCacheKeyCookie;
        }

        /// <summary>
        /// Gets the cookie for the current Add-In by looking at the current URL.
        /// </summary>
        /// <param name="httpContext">The current Http context.</param>
        /// <returns></returns>
        public static HttpCookie GetCookieForAddin(HttpContextBase httpContext)
        {
            HttpCookie spCacheKeyCookie = null;
            string currentUrl = httpContext.Request.Url.ToString();
            AddinSection section = (AddinSection)ConfigurationManager.GetSection("addinSection");
            AddIn addin = section.Addins.Cast<AddIn>().First(a => currentUrl.ToLower().Contains(a.Url.ToLower()));

            spCacheKeyCookie = httpContext.Request.Cookies[addin.Name];

            return spCacheKeyCookie;

        }
    }
}