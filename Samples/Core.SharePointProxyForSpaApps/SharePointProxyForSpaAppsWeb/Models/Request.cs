using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace SharePointProxyForSpaAppsWeb.Models
{
    public class Request
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public JObject Headers { get; set; }
        public string Data { get; set; }
    }
}