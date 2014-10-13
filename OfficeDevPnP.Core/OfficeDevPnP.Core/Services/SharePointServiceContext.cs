using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Services
{
    public class SharePointServiceContext
    {
        public string CacheKey { get; set; }
        public string Token { get; set; }
        public string HostWebUrl { get; set; }
        public string AppWebUrl { get; set; }
        public string ClientId { get; set; }
        public string HostedAppHostName { get; set; }
    }
}
