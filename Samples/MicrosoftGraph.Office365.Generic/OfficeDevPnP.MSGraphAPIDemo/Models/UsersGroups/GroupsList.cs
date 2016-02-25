using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a list of groups
    /// </summary>
    public class GroupsList
    {
        /// <summary>
        /// The list of contacts
        /// </summary>
        [JsonProperty("value")]
        public List<Group> Groups { get; set; }
    }
}