using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    public class ConversationsList
    {
        /// <summary>
        /// The list of threads
        /// </summary>
        [JsonProperty("value")]
        public List<Conversation> Conversations { get; set; }
    }
}