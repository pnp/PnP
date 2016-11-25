using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines the location of an event
    /// </summary>
    public class EventLocation
    {
        /// <summary>
        /// The display name of the location for the event
        /// </summary>
        [JsonProperty("displayName")]
        public String Name { get; set; }
    }
}