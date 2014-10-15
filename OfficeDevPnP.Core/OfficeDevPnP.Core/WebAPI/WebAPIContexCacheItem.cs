using Microsoft.IdentityModel.S2S.Protocols.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.WebAPI
{
    /// <summary>
    /// This class holds the information that's being cached as part of the WebAPI service implementation
    /// </summary>
    public class WebAPIContexCacheItem
    {
        /// <summary>
        /// The SharePoint Access token
        /// </summary>
        public OAuth2AccessTokenResponse AccessToken { get; set; }
        /// <summary>
        /// The SharePoint Refresh token
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// The information initially used to register the SharePoint app to the WebAPI service
        /// </summary>
        public WebAPIContext SharePointServiceContext { get; set; }
    }
}
