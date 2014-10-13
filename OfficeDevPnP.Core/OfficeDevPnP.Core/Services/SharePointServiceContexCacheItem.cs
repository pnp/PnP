using Microsoft.IdentityModel.S2S.Protocols.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Services
{
    public class SharePointServiceContexCacheItem
    {
        public OAuth2AccessTokenResponse AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public SharePointServiceContext SharePointServiceContext { get; set; }
    }
}
