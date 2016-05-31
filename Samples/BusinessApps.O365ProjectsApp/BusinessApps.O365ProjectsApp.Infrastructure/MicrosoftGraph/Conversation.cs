using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessApps.O365ProjectsApp.Infrastructure.MicrosoftGraph
{
    public class Conversation : BaseModel
    {
        public String Topic { get; set; }
        public List<ConversationThread> Threads { get; set; }
        public Boolean HasAttachments { get; set; }
        public DateTimeOffset LastDeliveredDateTime { get; set; }
        public List<String> UniqueSenders { get; set; }
        public String Preview { get; set; }
    }
}