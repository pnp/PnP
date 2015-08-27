using System;
using System.Net;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Utilities
{
    public static class Utility
    {
        /// <summary>
        /// Check if the property is loaded on the site object, if not the site object will be reloaded
        /// </summary>
        /// <param name="cc">Context to execute upon</param>
        /// <param name="site">Site to execute upon</param>
        /// <param name="propertyToCheck">Property to check</param>
        /// <returns>A reloaded site object</returns>
        public static Site EnsureSite(ClientRuntimeContext cc, Site site, string propertyToCheck)
        {
            if (!site.IsObjectPropertyInstantiated(propertyToCheck))
            {
                // get instances to root web, since we are processing currently sub site 
                cc.Load(site);
                cc.ExecuteQueryRetry();
            }
            return site;
        }

        /// <summary>
        /// Check if the property is loaded on the web object, if not the web object will be reloaded
        /// </summary>
        /// <param name="cc">Context to execute upon</param>
        /// <param name="web">Web to execute upon</param>
        /// <param name="propertyToCheck">Property to check</param>
        /// <returns>A reloaded web object</returns>
        public static Web EnsureWeb(ClientRuntimeContext cc, Web web, string propertyToCheck)
        {
            if (!web.IsObjectPropertyInstantiated(propertyToCheck))
            {
                // get instances to root web, since we are processing currently sub site 
                cc.Load(web);
                cc.ExecuteQueryRetry();
            }
            return web;
        }

        /// <summary>
        /// Returns the healthscore for a SharePoint Server
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static int GetHealthScore(string url)
        {
            int value = 0;
            Uri baseUri = new Uri(url);
            Uri checkUri = new Uri(baseUri, "_layouts/15/blank.htm");
            WebRequest webRequest = WebRequest.Create(checkUri);
            webRequest.Method = "HEAD";
            webRequest.UseDefaultCredentials = true;
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                foreach (string header in webResponse.Headers)
                {
                    if (header == "X-SharePointHealthScore")
                    {
                        value = Int32.Parse(webResponse.Headers[header]);
                        break;
                    }
                }
            }
            return value;
        }

        //public static string URLCombine(string baseUrl, string relativeUrl)
        //{
        //    if (baseUrl.Length == 0)
        //        return relativeUrl;
        //    if (relativeUrl.Length == 0)
        //        return baseUrl;
        //    return string.Format("{0}/{1}", baseUrl.TrimEnd(new char[] { '/', '\\' }), relativeUrl.TrimStart(new char[] { '/', '\\' }));
        //}


    }
}
