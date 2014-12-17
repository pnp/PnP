using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.DisplayCalendarEventsWeb.Models {
    public class Request {
        public string Url { get; set; }
        public string Method { get; set; }
        public JObject Headers { get; set; }
        public JObject Data { get; set; }
    }
}