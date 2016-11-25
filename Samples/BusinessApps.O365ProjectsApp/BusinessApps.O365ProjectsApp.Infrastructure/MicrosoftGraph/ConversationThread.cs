using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessApps.O365ProjectsApp.Infrastructure.MicrosoftGraph
{
    public class ConversationThread : BaseModel
    {
        /// <summary>
        /// The list of email address TO recipients
        /// </summary>
        [JsonProperty("toRecipients")]
        public List<UserInfoContainer> To { get; set; }

        public List<ConversationThreadPost> Posts { get; set; }

        public String Topic { get; set; }

        public Boolean HasAttachments { get; set; }

        public DateTimeOffset LastDeliveredDateTime { get; set; }

        public List<String> UniqueSenders { get; set; }

        /// <summary>
        /// The list of email address CC recipients
        /// </summary>
        [JsonProperty("ccRecipients")]
        public List<UserInfoContainer> CC { get; set; }

        public String Preview { get; set; }

        public Boolean IsLocked { get; set; }
    }
}