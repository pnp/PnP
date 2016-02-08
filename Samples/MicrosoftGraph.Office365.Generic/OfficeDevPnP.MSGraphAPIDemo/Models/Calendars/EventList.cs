using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{ 
    /// <summary>
    /// Defines a list of calendar's events
    /// </summary>
    public class EventList
    {
        /// <summary>
        /// The list of calendar's events
        /// </summary>
        [JsonProperty("value")]
        public List<Event> Events { get; set; }
    }
}