using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.WebAPI
{
    /// <summary>
    /// This class holds the information that's passed from the SharePoint app to the "Register" WebAPI service call
    /// </summary>
    public class WebAPIContext
    {
        /// <summary>
        /// The cacheKey that will be used. The cache key is unique for each combination of user name, user name issuer, application, and farm. 
        /// </summary>
        public string CacheKey { get; set; }
        /// <summary>
        /// The SharePoint context token. This will be used at the WebAPI level to obtain an access token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Url of the SharePoint host web. Needed to obtain an access token
        /// </summary>
        public string HostWebUrl { get; set; }
        /// <summary>
        /// Url if the SharePoint app web. Needed to obtain an access token
        /// </summary>
        public string AppWebUrl { get; set; }
        /// <summary>
        /// ClientId of the SharePoint app that's being registered. Needed to obtain an access token
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// If the AppWebUrl is null then this value will be used. Needed to obtain an access token
        /// </summary>
        public string HostedAppHostName { get; set; }
    }
}
