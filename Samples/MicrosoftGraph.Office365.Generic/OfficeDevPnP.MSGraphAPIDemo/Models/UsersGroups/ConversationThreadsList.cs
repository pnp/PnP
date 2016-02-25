using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a list of threads
    /// </summary>
    public class ConversationThreadsList
    {
        /// <summary>
        /// The list of threads
        /// </summary>
        [JsonProperty("value")]
        public List<ConversationThread> Threads { get; set; }
    }
}