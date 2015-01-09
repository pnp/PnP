using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Demo
{
    public static class Office365ServicesUris
    {
        public static readonly Uri DiscoveryServiceEndpointUri = new Uri("https://api.office.com/discovery/v1.0/me/");

        public static readonly string DiscoveryServiceResourceId = "https://api.office.com/discovery/";

        public static readonly string AADGraphAPIResourceId = "https://graph.windows.net";
    }
}
