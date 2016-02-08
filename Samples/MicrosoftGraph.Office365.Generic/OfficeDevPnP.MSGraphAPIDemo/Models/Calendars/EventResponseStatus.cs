using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines the Response Status of a Meeting Request
    /// </summary>
    public class EventResponseStatus
    {
        /// <summary>
        /// The type of response
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ResponseType Response { get; set; }

        /// <summary>
        /// The date and time of response
        /// </summary>
        [JsonProperty("time")]
        public DateTime ResponseDateTime { get; set; }
    }

    public enum ResponseType
    {
        None = 0,
        Organizer = 1,
        TentativelyAccepted = 2,
        Accepted = 3,
        Declined = 4,
        NotResponded = 5,
    }
}