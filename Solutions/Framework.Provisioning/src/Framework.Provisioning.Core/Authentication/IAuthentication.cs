using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core.Authentication
{
    /// <summary>
    /// Interface that is used to implement Authentication Class
    /// </summary>
    public interface IAuthentication
    {
        /// <summary>
        /// The tenant admin Url for the environment.
        /// </summary>
        string TenantAdminUrl
        {
            get;
        }

        /// <summary>
        /// The Site Url
        /// </summary>
        string SiteUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Returns am Authenticated ClientContext
        /// </summary>
        /// <returns></returns>
        ClientContext GetAuthenticatedContext();

        /// <summary>
        /// Gets an HttpWebRequest that is Authenticated
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        HttpWebRequest GetAuthenticatedWebRequest(string url);
    }
}
