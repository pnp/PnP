using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Authentication
{
    /// <summary>
    /// Interface that is used to implement Authentication Class
    /// </summary>
    public interface IAuthentication
    {
        /// <summary>
        /// Gets or sets the TenantAdminUrl
        /// </summary>
        string TenantAdminUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the SiteUrl
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
