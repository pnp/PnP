using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a list of email message folders
    /// </summary>
    public class CalendarList
    {
        /// <summary>
        /// The list of email message folders
        /// </summary>
        [JsonProperty("value")]
        public List<Calendar> Calendars { get; set; }
    }
}